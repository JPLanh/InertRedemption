'use strict';

var mongoose = require('mongoose'),
	user = mongoose.model("User"),
	client = mongoose.model("Client"),
	table = mongoose.model("Table"),
	crypto = require('crypto'),
	fs = require('fs'),
	jwt = require('jsonwebtoken'),
	algorithm = 'aes-256-cbc';

exports.joinGame = async function(req, res){
	var getAction = req.body.action;
	if (getAction == 'Join'){
		console.log("join happened");
		user.findOneAndUpdate({'username': req.body.oldUsername}, {'username': req.body.username, 'table': 1}, function(err, updateUser){});
		res.send(req.body.username + " has joined the game.");
	} else if (getAction == 'Draw'){
		console.log("Draw happened");
		var userRet;
		user.findOne({"username": req.body.username}).exec()
		.then(function(getUser){
			userRet = getUser;
			return table.findOne({"number": getUser.table}).exec();
		})
		.then(function(getTable){
			var card = getTable.deck.shift();
			getTable.state = "Draw";
			getTable.save();
			return card;
		})
		.then(function(getCard){
			userRet.hand.push(getCard);
			userRet.save();
			return getCard;
		})
		.then(function(getCard){
			return res.send(getCard);
//			return res.json({"Username": userRet.username, "Card": getCard});
		});
		
	} else if (getAction == 'Quit'){
		user.deleteOne({"username": req.body.username}, function(err, deleted){});
		res.send("Deleted");
	} else if (getAction == 'Change state'){
		user.updateOne({'username': req.body.username},{'state': req.body.state}, function(err, updateUser){
			if (req.body.state == "New game") {
				gameReadyCheck();
			}

		});
		res.send("Updated user");
	}
};

exports.finishAction = async function(getUsername, getTableID, getSocket) {
	let getClient = client.findOne({"loginUser.username": getUsername}).exec();
	let getTable = table.findOne({"tableID": getTableID}).exec();
	return await Promise.all([getClient, getTable])
	.then(async (ret) => {
	//	console.log(ret);
		ret[0]["Turn"] = ret[0]["Turn"] - 1;
		if (ret[0]["Turn"] < 1){
			let tmpPayload = {};
			tmpPayload["Deck size"] = ret[1]['deck'].length
			ret[0]["Turn"] = 1;
			getSocket.in(ret[0]["tableID"]).emit("Action", {"Action": "Finish Turn", "Username": getUsername, "Payload": tmpPayload});
		}
		return ret[0].save();

	});
//	return await client.findOne({"loginUser.username": getUsername}).exec()
//	.then(async (getClient) => {
//	});
}

exports.draw = async function(getTable, getUser) {
	return await client.findOne({"loginUser.username": getUser}).exec()
	.then(async (foundUser) => {
		var card = getTable.deck.shift();
		foundUser.loginUser.hand.push(card);
		await foundUser.save();
		return card;
	})
	.then(async (getCard) =>{
		await getTable.save()
		return getCard;
	});
}

exports.nextPlayer = async function(getTable, getUser) {
	let index = getTable.order.indexOf(getUser);
	getTable['activeUser'] = getTable.order[(index+1)%getTable.order.length];
	return await getTable.save();
}

exports.cardPossabilities = async function(getHand, getPlayed) {
	console.log(getHand);
	getHand.sort();
	var possabilties = getPossabilities(getHand, getPlayed);
	
}

function getPossabilities(getHand, getPlayed){
	var currentIndex = 0;
	var currentCard = getHand[0];
	var possabilities = [];
	for ( var index = 0; index < getHand.length; index++){
		var combo = [];
		if (combo.length == 0) {
			combo.push(currentCard);
			currentIndex = index;
		}
		else {
			switch(getType(currentCard)){
	       		 	case "Blue":
            			case "Yellow":
            			case "Purple":
            			case "Orange":
					if (getType(currentIndex) == getType(index)) combo.push(currentCard);
					else {
						combo = [];
						currentIndex = index;
						currentCard = getHand[index];
						combo.push(currentCard);
					}
					break;
				case "Complementary Cookie":
					switch(getType(getHand[index])){
						case "Green":
				            	case "Orange":
						//need played
					}
					break;
			}
		}
		possabilities.push(combo);
	}
}

function getType(value)
    {
        switch (value)
        {
            case 0:
            case 1:
                return "Blue";
            case 2:
                return "Brown";
            case 3:
                return "Complementary Cookie";
            case 4:
                return "Drink Carrier";
            case 5:
                return "Drink Spill";
            case 6:
            case 7:
            case 8:
                return "Green";
            case 9:
                return "Loyalty Card";
            case 10:
            case 11:
            case 12:
                return "Orange";
            case 13:
            case 14:
                return "Purple";
            case 15:
                return "To-Go Cup";
            case 16:
            case 17:
            case 18:
                return "Yellow";
            default:
                return null;
        }
    }

function gameReadyCheck(){
	user.count({}, function(err, userCount){
		user.count({"state": "New game"}, function(err, playerReady){
		if (userCount == playerReady) {
			table.updateOne({}, {"state": "Preparing"}, function(err, updated){
	});
		}
		});
	});
}

exports.getPlayers = function(req, res){
	user.find({}, function(err, foundUser){
		res.json(foundUser);
	});
}

exports.resetClient = async function(getUserID, getUser, getSocket) {
	return await client.findOne({"userID": getUserID}).exec()
	.then(async (getClient) => {
                getClient["loginUser"] = getUser;
                getClient["State"] = "Main Lobby";
                getClient["lastPlayed"] = null;
                getClient["played"] = [];
                getClient["Turn"] = 1;
		getClient["tableID"] = "Main Lobby";
		getSocket.join("Main Lobby");
		return await getClient.save();

	});
};

exports.emptyTableCheck = async function(username, getTable, socket) {
	return await client.countDocuments({"tableID": getTable}).exec()
	.then(async (getCount) => {
		console.log(getCount + " " + getTable);
		if (getCount < 2 && getTable != "Main Lobby"){
			socket.in("Main Lobby").emit('Lobby', {"Action": "Room Closed", "Room": getTable});           
			socket.in(getTable).emit('Action', {"Action": "Game Over", "Room": getTable});           
			return table.deleteOne({"tableID": getTable});
		}
	});
};

exports.newGame = async function(host, order, getRoom){
	console.log("new game");
	var mapper = {
		"blue1":{"index":0, "amount":5}, 
		"blue2":{"index":1, "amount":5},
		"brown":{"index":2, "amount":4},
		"compCookie":{"index":3, "amount":4},
		"drinksCarrier":{"index":4, "amount":2},
		"drinkSpill":{"index":5, "amount":4},
		"green2":{"index":6, "amount":5},
		"green3":{"index":7, "amount":3},
		"green4":{"index":8, "amount":2},
		"loyaltyCard":{"index":9, "amount":2},
		"orange2":{"index":10, "amount":5},
		"orange3":{"index":11, "amount":4},
		"orange4":{"index":12, "amount":3},
		"purp1":{"index":13, "amount":10},
		"purp2":{"index":14, "amount":5},
		"toGoCup":{"index":15, "amount":4},
		"yellow1":{"index":16, "amount":5},
		"yellow2":{"index":17, "amount":5},
		"yellow3":{"index":18, "amount":5}
	};

	var drawPile = [];
	Object.keys(mapper).forEach(
		function(key) {
			for (var cardAmt = 0; cardAmt < mapper[key].amount; cardAmt++)
				drawPile.push(mapper[key].index);
		}
	);
	shuffle(drawPile);
	shuffle(drawPile);
	shuffle(order);
	shuffle(order);

	let getTable = await new table({"tableID": getRoom, "deck": drawPile, "order": order, "activeUser": order[0], "gameHost": host, "state": "waiting"}).save();

	return getTable;
}

function shuffle(array){
	var currentIndex = array.length, temporaryValue, randomIndex;

  // While there remain elements to shuffle...
	while (0 !== currentIndex) {

    // Pick a remaining element...
 	randomIndex = Math.floor(Math.random() * currentIndex);
 	currentIndex -= 1;

    // And swap it with the current element.
 	temporaryValue = array[currentIndex];
 	array[currentIndex] = array[randomIndex];
  	array[randomIndex] = temporaryValue;
	}
}
exports.postUpdate = function(req, res){
  updates({"message": req.query.msg, "category": req.query.cata, "posted": (new Date())}).save();	
};

exports.getGameUpdates = function(req, res){
	updates.findOne({category: "Game"}).sort({posted: -1}).exec(function(err, getUpdate){
		res.json(getUpdate);
	});
};

exports.getLifeUpdates = function(req, res){
	updates.findOne({category: "Life"}).sort({posted: -1}).exec(function(err, getUpdate){
		res.json(getUpdate);
	});
};

exports.ping = function(req, res){
	res.send("ping");
};

exports.sendCommands = function(req, res){
	command.getCommands(req.query.command, req.query.username, function(callBack){
		res.send(callBack);
	});
};

exports.addItem = function(req, res){
	item.findOne({'name': req.params.name}, function(err, foundItem){
		if (!foundItem){
			var newItem = new item({'name': req.params.name}).save();
			res.send(req.params.name + " has been created");
		}
	});
};

exports.getRoomTable = function(req, res){
	user.findOne({'username': req.params.username}, function(err, foundUser){
		table.find({'inRoom.name': foundUser.inRoom[0].name}, function(err, foundRoom){
			res.json(foundRoom);
		});
	});
};

exports.createRoom = function(req, res){
	var newRoom = new room(req.query);
	console.log(newRoom);
	newRoom.save(function(err, roomSaved){
		res.send("Room created");
	});
};

exports.enterRoom = function(req, res){
	room.findOne({"name": req.params.username}, function(err, foundRoom){
		user.findOne({"username": req.query.username}, function(err, foundUser){
			foundUser.room = foundRoom;
			user.findOneAndUpdate({"username":req.query.username }, foundUser, {upsert:true, overwrite:true}, function(err, updateUser)
			{
				res.send("Entered Room");
			});	
		});
	});
};

exports.newUser = function(req, res){
	var newUser = new user(req.query);
	console.log(newUser);
	room.findOne({'name': "Void"}, function(err, foundRoom){
		newUser.inRoom = foundRoom;
		serverkey.findOne({}, function(err, keyFound){	
			var cryptoKey = Buffer.from(keyFound.key, 'hex');
			var cryptoIv = Buffer.from(keyFound.iv, 'hex');
			let cipher = crypto.createCipheriv('aes-256-cbc', cryptoKey, cryptoIv);
			let cipherText = cipher.update(newUser.password + "A very hardcoded delicious salt");
			cipherText = Buffer.concat([cipherText, cipher.final()]);
			newUser.password = cipherText.toString('hex');
			newUser.save(function(err, userSaved){
				res.json(userSaved);
			});
		});
	});

};


exports.existingUser = function(req, res){
	console.log("Tseting");
	serverkey.findOne({}, function(err, keyFound){	
		var cryptoKey = Buffer.from(keyFound.key, 'hex');
		var cryptoIv = Buffer.from(keyFound.iv, 'hex');
		let cipher = crypto.createCipheriv('aes-256-cbc', cryptoKey, cryptoIv);
		let cipherText = cipher.update(req.query.password + "A very hardcoded delicious salt");
		cipherText = Buffer.concat([cipherText, cipher.final()]);
		console.log("Testing");
		user.findOne({'username': req.query.username, 'password': cipherText.toString('hex')}, function(err, userFound){
			console.log("Hello");
			if (userFound == null) res.status(404).send({auth: false});
			else {
				userFound.active = true;
				console.log(userFound);
				console.log(config);
				user.findOneAndUpdate({'username': req.query.username}, userFound, {upsert:true, overwrite:true}, function(err, updateUser){
					var newToken = jwt.sign({id: userFound._id}, config["TokenSecret"], {expiresIn: 30});
					var newRefresh = jwt.sign({id: userFound._id}, config["RenewSecret"], {expiresIn: config["RenewLife"]});
					jwt.verify(newToken, config["TokenSecret"], function(err, decoded) {
  						new login({"id": decoded.id, 'exp': decoded.exp}).save();
						res.status(200).send({ auth: true, access: getAccess(userFound.username), token: newToken, refresh: newRefresh, exp: decoded.exp });
					});
//					res.send(newToken);
				});
						}//res.send("Success");
		});
	});
};

function getAccess(username){
	var headerStr = "";
	headerStr += "<ul ng-click='homeButton()'>Home</ul>";
	headerStr += "<ul ng-click='myProjectsButton()'>My Projects</ul>";
	headerStr += "<ul ng-click='aboutMeButton()'>About Me</ul>";
	headerStr += "<ul ng-click='myAcademicButton()'>My Academic</ul>";
	headerStr += "<ul ng-click='myExperienceButton()'>My Work Experience </ul>";
	headerStr += "<br>Member:<br>";
	if (username){	  
	  headerStr += "<ul ng-click='myGameButton()'>Game</ul>";
	  headerStr += "<ul ng-click='logoffButton()Button()'>Logoff</ul>";
	  if (username == "JPLanh"){
	    headerStr += "<ul ng-click='userButton()'>Post<ul>";
	    }
	} else {
	headerStr += "<ul ng-click='loginButton()'>Login</ul>";
	}
	return headerStr;
}

exports.getRoomInfo = function(req, res){
	user.findOne({'username': req.params.username}, function(err, foundUser)	
	{
		user.find({'inRoom.name': foundUser.inRoom[0].name, 'active':true}, function(err, userList)
		{
			humanoid.find({'inRoom.name': foundUser.inRoom[0].name}, function(err, npcList)
			{
				chat.find({'listener.username': foundUser.inRoom[0].name}, function(err, foundChat)
				{
					res.status(200).send({users: userList, npcs: npcList, chats: foundChat });
/*					if (foundChat.length > 0){
						chat.deleteMany({'listener.username': req.params.username}, function(err, deleted){});
						res.json(foundChat);
					} else {
						res.send("Empty");
					}*/
				});
			});		
		})
	});
}

exports.getUserRoom = function(req, res){
	user.findOne({'username': req.params.username}, function(err, foundUser){
		res.send(foundUser.inRoom);
	});
};
exports.getCommand = function(req, res){
	res.send("Complete");
};

exports.getChat = function(req, res){
	chat.find({'listener.username': req.params.username}, function(err, foundChat){
		if (foundChat.length > 0){
			chat.deleteMany({'listener.username': req.params.username}, function(err, deleted){});
			res.json(foundChat);
		} else {
			res.send("Empty");
		}
	});
};

exports.getNPC = function(req, res){
	user.findOne({'username': req.params.username}, function(err, foundUser){
		humanoid.find({'inRoom.name': foundUser.inRoom[0].name}, function(err, npcList){
			res.json(npcList);
		});
	});
}

exports.getToken = function(req, res){
  	jwt.verify(req.headers['x-refresh-token'], config["RenewSecret"], function(err, newDecoded) {
		if (newDecoded){
			var newToken = jwt.sign({"id": newDecoded.id}, config["TokenSecret"], {expiresIn: config["TokenLife"]});
		  	jwt.verify(newToken, config["TokenSecret"], function(err, decoded){
				login.findOne({"id": decoded.id}, function(err, foundLogin){
					foundLogin.exp = decoded.exp;
					login.findOneAndUpdate({"id": decoded.id}, foundLogin, {upsert:true, overwrite:true}, function(err, updateLogin){
						res.status(200).send({ auth: true, token: newToken, exp: decoded.exp });
					});
				});
			});
		} else {
			res.status(403).send({ auth: false, message: "Expired token"});
		}
	});
}

exports.postChat = function(req, res){
	user.findOne({'username': req.params.username}, function(err, foundUser){
		var newChat = new chat({'inRoom.name': foundUser.inRoom[0].name, 'text': req.query.message}); 
		console.log("Saved: " + req.query.message);
		newChat.save(function(saveChat){
			res.send("Chat sent");
		});
	});
};

exports.getRoomExits = function(req, res){
	user.findOne({'username': req.params.username}, function(err, foundUser){
		rmCon.find({"fromRoom.name": foundUser.inRoom[0].name}, function(err, foundExits){
			if (foundExits){
				res.json(foundExits)
			}
		});
	});
}
