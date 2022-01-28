const models = require('../Model/gameModel'),
	controller = require('../Controller/gameController'),
	tableController = require('../Controller/TableController'),
	loginController = require('../Controller/LoginController'),
	crypto = require('crypto'),
	mongoose = require('mongoose'),
	user = mongoose.model('User'),
	table = mongoose.model('Table'),
	memory = require('../../Memory/memory'),
	client = mongoose.model('Client');

module.exports = async function(socket){
	socket.on('connect', async (getSocket) => {
		var message = {}
		console.log('a user connected');
		new client({"userID": getSocket.id}).save();

		getSocket.on('disconnecting', async () => {
			loginController(socket, getSocket);	
//			let getClient = await client.findOne({"userID": getSocket.id}).exec();
//			if (getClient.loginUser != null) socket.to(getClient['tableID']).emit('Action', {"Action": "Exit", "Username": getClient.loginUser['username']});
		});
	
	  	getSocket.on('disconnect', async () => {
			loginController(socket, getSocket);
//			const getClient = await client.findOne({"userID": getSocket.id}).exec();
//			const delRes = await client.deleteOne({"userID": getSocket.id});
//			if (getClient.loginUser != null) await controller.emptyTableCheck(getClient.loginUser["username"], getClient.tableID, socket);
    			console.log('user disconnected');
		});
 
		getSocket.on('register', async (getUserName, getPassword) => {
			let findUser = user.findOne({"username": getUserName}).exec();
			let findClient = client.findOne({"userID": getSocket.id}).exec();

			Promise.all([findUser, findClient])
			.then(async (res) => {
				//console.log(res);
				if (res[0] != null) {
					getSocket.emit('Action', {"Action": "Error", "Username": res[0]['username'], "Message": "User already exists"});
				} else {
					var new_key = {key: (crypto.randomBytes(32)).toString('hex'), iv: (crypto.randomBytes(16)).toString('hex')}
					var cryptoKey = Buffer.from(new_key.key, 'hex');
					var cryptoIv = Buffer.from(new_key.iv, 'hex');
					let cipher = crypto.createCipheriv('aes-256-cbc', cryptoKey, cryptoIv);
					let cipherText = cipher.update(getPassword + "A very hardcoded delicious salt");
					cipherText = Buffer.concat([cipherText, cipher.final()]);
					let password = cipherText.toString('hex');
					res[0] = await new user({"username": getUserName, "password": password, "key": new_key.key, "iv": new_key.iv}).save();
					return res;	
				}
			})
			.then(async (res) => {
				getSocket.emit('Login', {"Action": "Welcome", "Username": res[0]['username']});
				getSocket.nickname = res[0]['username'];
				getSocket.join("Main Lobby");
				let resetStatus = await controller.resetClient(getSocket.id, res[0], getSocket); 
			})				
		});
	
		getSocket.on('login', async (getUserName, getPassword) => {
			let findUser = user.findOne({"username": getUserName}).exec();
			let findClient = client.findOne({"userID": getSocket.id}).exec();
			console.log("Someone logging in");	
			Promise.all([findUser, findClient])
			.then(async (res) => {
				if (res[0] == null) {
					getSocket.emit('Login', {"Action": "Bad Credential", "Message": "Bad Credentials"});
					console.log("Bad credential");
				} else {
					console.log("Good credential");
					var cryptoKey = Buffer.from(res[0]['key'], 'hex');
					var cryptoIv = Buffer.from(res[0]['iv'], 'hex');
					let cipher = crypto.createCipheriv('aes-256-cbc', cryptoKey, cryptoIv);
					let cipherText = cipher.update(getPassword + "A very hardcoded delicious salt");
					cipherText = Buffer.concat([cipherText, cipher.final()]);
					let password = cipherText.toString('hex');
					if (password == res[0]['password']){
						let resetStatus = await controller.resetClient(getSocket.id, res[0], getSocket); 
						getSocket.emit('Login', {"Action": "Welcome", "Username": res[0]['username']});
						getSocket.join("Main Lobby");
						getSocket.nickname = res[0]['username'];
					} else {
						getSocket.emit('Login', {"Action": "Bad Credential", "Username": res[0]['username'], "Message": "Invalid user credentials"});
					}
				}
			})
		});	

		getSocket.on("Register", async (getAction, getUserName, getPassword) => {
			switch(getAction){
				case "Check":
					return await user.countDocuments({"username": getUserName}).exec()
					.then((getUser) => {
					if (getUser > 0) 
						getSocket.emit('Register', {"Action": "User Exists", "Username": getUserName, "Message": "User currently exists"});
					else
						getSocket.emit('Register', {"Action": "Register Proceed", "Username": getUserName, "Message": getPassword});
					});
					break;
				case "Validate":
					console.log("Validating");
					await client.findOne({"userID": getSocket.id}).exec()
					.then(async (getClient) => {
						var new_key = {key: (crypto.randomBytes(32)).toString('hex'), iv: (crypto.randomBytes(16)).toString('hex')}
						var cryptoKey = Buffer.from(new_key.key, 'hex');
						var cryptoIv = Buffer.from(new_key.iv, 'hex');
						let cipher = crypto.createCipheriv('aes-256-cbc', cryptoKey, cryptoIv);
						let cipherText = cipher.update(getPassword + "A very hardcoded delicious salt");
						cipherText = Buffer.concat([cipherText, cipher.final()]);
						let password = cipherText.toString('hex');
						await new user({"username": getUserName, "password": password, "key": new_key.key, "iv": new_key.iv}).save();
						getSocket.emit('Login', {"Action": "Login Proceed", "Username": getUserName, "Message": getPassword});
					})
					break;
			}
		});


		getSocket.on("Login", async (getAction, getUserName, getPassword) => {
			switch(getAction){
				case "Check":
					return await client.countDocuments({"loginUser.username": getUserName}).exec()
					.then((getClient) => {
					if (getClient > 0) 
						getSocket.emit('Login', {"Action": "User Active", "Username": getUserName, "Message": "User is currently logged on."});
					else
						getSocket.emit('Login', {"Action": "Login Proceed", "Username": getUserName, "Message": getPassword});
					});
					break;
				case "Validate":
					console.log("Validating");
					let findUser = await user.findOne({"username": getUserName}).select("+password +iv +key").exec();
					let findClient = await client.findOne({"userID": getSocket.id}).exec();
					Promise.all([findUser, findClient])
					.then(async (res) => {
						if (res[0] == null) {
							getSocket.emit('Login', {"Action": "Bad Credential"});
						} else {
							var cryptoKey = Buffer.from(res[0]['key'], 'hex');
							var cryptoIv = Buffer.from(res[0]['iv'], 'hex');
							let cipher = crypto.createCipheriv('aes-256-cbc', cryptoKey, cryptoIv);
							let cipherText = cipher.update(getPassword + "A very hardcoded delicious salt");
							cipherText = Buffer.concat([cipherText, cipher.final()]);
							let password = cipherText.toString('hex');
							if (password == res[0]['password']){
								getSocket.emit('Login', {"Action": "Validated", "Username": getUserName});
							} else {
								getSocket.emit('Login', {"Action": "Bad Credential"});
							}
						}
					})
					break;
				case "Log In":
					return await user.findOne({"username": getUserName}).select(['-__v', '-_id']).exec()
					.then(async (getUser) => {
						let resetStatus = await controller.resetClient(getSocket.id, getUser, getSocket); 
//						console.log(resetStatus);
						getSocket.nickname = getUserName;
						getSocket.emit('Login', {"Action": "Welcome", "Username": getUserName});
					});

					break;
			}
		});

		getSocket.on("Lobby", async (getAction, getUserName, getRoom, getParams) => {
			let params = JSON.parse(getParams);
			switch(getAction){
			case "Create":
				getSocket.leave("Main Lobby");
				await new Promise((resolve, reject) => {
					client.findOne({"userID": getSocket.id}).exec()
					.then ((getClient) => {
						getSocket.join(getRoom);
						resolve(getClient);
					});
				}).then(async (getClient) => {
					let userList = [];
					new Promise((resolve, reject) => {
						Object.keys(socket.sockets.adapter.rooms[getRoom].sockets).forEach((perClient) => {
							userList.push(socket.sockets.connected[perClient].nickname);
						});
					})
					getClient["tableID"] = getRoom;
					getClient["State"] = "Table Lobby";
					getClient.save();
					socket.in(getRoom).emit('Lobby', {"Action": "Create", "Username": getUserName, "Room": getRoom, "UserList": userList});
					return userList;
				}).then(async (listOfClient) => {
					socket.in("Main Lobby").emit('Lobby', {"Action": "New Room", "Room": getRoom});
					return await controller.newGame(getUserName, listOfClient, getRoom)
				});
				break;
			case "Join":
				getSocket.leave("Main Lobby");
				let clientList = await new Promise((resolve, reject) => {
					client.findOne({"userID": getSocket.id}).exec()
					.then ((getClient) => {
						getSocket.join(getRoom);
						resolve(getClient);
					});
				}).then(async (getClient) => {
					let userList = [];
					new Promise((resolve, reject) => {
						Object.keys(socket.sockets.adapter.rooms[getRoom].sockets).forEach((perClient) => {
							userList.push(socket.sockets.connected[perClient].nickname);
						});
					})
					getClient["tableID"] = getRoom;
					getClient["State"] = "Table Lobby";
					getClient.save();
					return userList;
				}).then(async (listOfClients) => {
					socket.in(getRoom).emit('Action', {"Action": "Join", "Username": getUserName, "Room": getRoom, "UserList": listOfClients});
					return listOfClients;
				});
				table.findOne({"tableID": getRoom}).exec()
				.then(async (getTable) => {
					getTable['order'] = clientList;
					return await getTable.save();
				});
				break;
			case "Begin":
				table.findOne({"tableID": getRoom}).exec()
				.then(async (getTable) => {
					getTable["state"] = "New";
					await getTable.save();
				});
				socket.in(getRoom).emit('Action', {"Action": "Begin Game", "Username": getUserName, "Room": getRoom});
				break;
			case "Refresh Tables":
				var tableList = [];
				table.find({"state": "waiting"}).exec()
				.then(async (getTables) => {
					return await new Promise((resolve, reject) => {
						let tempList = [];
						getTables.forEach((perTable) => {
							tempList.push(perTable.tableID);
						});
						resolve(tempList);
					});
				})
				.then((tableList) => {				
					getSocket.emit('Lobby', {"Action": "Refresh Tables", "UserList": tableList});
				});
				break;
			};
		});

		getSocket.on("Game", async (getAction, userName, getRoom, getParams) => {
			let params = JSON.parse(getParams);
			let Payload = {};
//			console.log(getParams);
			if (getAction == "Begin"){
				var userList = [];
				Object.keys(socket.sockets.adapter.rooms[getRoom].sockets).forEach((perClient) => {
					userList.push(socket.sockets.connected[perClient].nickname);
				});
				table.findOne({"tableID": getRoom}).exec()
				.then((getTable) => {
					socket.in(getRoom).emit('Action', {"Action": "Start Game", "Username": getTable['activeUser'], "Message": userName});
				});
			} else if (getAction == "Game Entered"){
				tableController.enterGame(socket, userName, getRoom);
/*				let readyCount = await client.findOne({"loginUser.username": userName}).exec()
				.then(async (getClient) => {
					getClient["State"] = "Preparing";
					await getClient.save();
				})
				.then(async () => {
					return new Promise((resolve, reject) => {
						let userList = [];
						Object.keys(socket.sockets.adapter.rooms[getRoom].sockets).forEach((perClient) => {
							userList.push(socket.sockets.connected[perClient].nickname);
						});
						resolve(userList);
					})
				}).then(async (listOfClients) => {
					socket.in(getRoom).emit('Action', {"Action": "Join", "Username": userName, "Room": getRoom, "UserList": listOfClients});
				})
				.then(async () => {
					return await client.countDocuments({"tableID": getRoom, "State": "Table Lobby"}).exec();
				});

				await table.findOne({"tableID": getRoom}).exec()
				.then(async (getTable) => {
					let tmpJson = {};
					tmpJson["State"] = "Preparing";
					//console.log("Count: " + readyCount);
					if (readyCount == 0){ 
						socket.in(getRoom).emit('Action', {"Action": "Prepare Game", "Username": userName, "Payload": tmpJson});
						getTable["state"] = "Prepare";
						getTable.save();
					}
				});*/
			} else if (getAction == "Draw"){
				//console.log(userName + ": drawing");
				return await table.findOne({"tableID": getRoom}).exec()
				.then(async (getTable) => {
					const val = await controller.draw(getTable, userName);
//					socket.in(getRoom).emit('Action', {"Action": "Draw", "Username": userName, "Value": val, "State": getTable["state"]});	
					getSocket.broadcast.to(getRoom).emit('Action', {"Action": "Draw", "Username": userName, "Value": -1, "State": getTable["state"]});
					getSocket.emit('Action', {"Action": "Draw", "Username": userName, "Value": val, "State": getTable["state"]});	
				});
			} else if (getAction == "Prepare Table") {
				let getUser = client.findOne({"loginUser.username": userName}).exec();
				let getTable = table.findOne({"tableID": getRoom}).exec();

				Promise.all([getUser, getTable])
				.then(async (ret) => {
					await tableController.prepareTable(socket, getSocket, ret[0], ret[1]);
				});
			} else if (getAction == "Next Turn"){
				//console.log(userName + ": Next Turn");
				await client.findOne({"loginUser.username": userName}).exec()
				.then(async (getClient) => {
					getClient["State"] = "Inactive";
					await getClient.save();
					return await table.findOne({"tableID": getRoom}).exec();
				})
				.then(async (getTable) => {
					let index = getTable.order.indexOf(userName);
					getTable['activeUser'] = getTable.order[(index+1)%getTable.order.length];
					await getTable.save();
					return await client.findOne({"loginUser.username": getTable['activeUser']}).exec();
				})
				.then(async (getNextClient) => {
					if (getNextClient["State"] != "Preparing") getNextClient["State"] = "Active";
					let payload = {};
					payload["State"] = getNextClient["State"];
					socket.in(getRoom).emit('Action', {"Action": "Next Turn", "Username": getNextClient.loginUser['username'], "Payload": payload});	
					return await getNextClient.save();
				}); 
/*
				let getClientTmp = await client.findOne({"loginUser.username": userName}).exec();
				let getTableTmp = await table.findOne({"tableID": getRoom}).exec();

				return Promise.all([getTableTmp, getClientTmp])
				.then(async (ret) => {
					let index = getTable.order.indexOf(getUser);
					getTable['activeUser'] = getTable.order[(index+1)%getTable.order.length];
					let upTable = await controller.nextPlayer(ret[0], userName);
					let payload = {};
					//console.log("Next Active: " + upTable['activeUser']);
					payload["State"] = "Active";
					socket.in(getRoom).emit('Action', {"Action": "Next Turn", "Username": upTable['activeUser'], "Payload": payload});	
					return ret;
				})
				.then(async (ret) => {
					ret[1]["State"] = "Inactive";
					return await ret[1].save();
				})*/
			} else if (getAction == "Get Suggestion"){
//				console.log(JSON.parse(getParams));
				client.find({"tableID": getRoom}).select('-userID -__v -State -Turn -tableID -_id -loginUser._id').exec()
				.then(async (getClient) => {
					//console.log(JSON.stringify(getClient));
//					console.log(JSON.parse(JSON.parse(getParams)["Hand List"]));
//					console.log(getClient.loginUser['hand']);
				});
//				controller.cardPossabilities(getClient.loginUser['hand']);
//				controller.cardPossabilities(['1','3','5','6','1'], []);
				return null;
			} else if (getAction == "Prepare Game"){
				//console.log(userName + ": Preparing Game" );
				let getTable = await table.findOne({"tableID": getRoom}).exec();
				let getClient = await client.findOne({"loginUser.username": userName}).exec();

				return Promise.all([getTable, getClient]).then((ret) => {
					let payload = {};
					payload["State"] = ret[1].State;
					socket.in(getRoom).emit('Action', {"Action": "Prepare Game", "Username": ret[0].activeUser, "Payload": payload});
				});
			} else if (getAction == "Prepare Done"){
				//console.log(userName + ": Preparing Done");
				table.findOne({"tableID": getRoom}).exec()
				.then(async (getTable) => {
					getTable["state"] = "Playing";
					return await getTable.save();
				});
			} else if (getAction == "Play Card"){
				//console.log(userName);
				tableController.playCard(socket, getSocket, userName, params["Play List"]);
/*
				let playList = JSON.parse(params["Play List"]);
//				console.log(params);
				return await client.findOne({"userID": getSocket.id}).exec()
				.then(async (getUser) => {
					return new Promise(async (resolve, reject) => {
						Payload = {};
						Payload["Special Action"] = "None";
						playList.forEach(async (cardIndex) => {
//							console.log(cardIndex);
							if (cardIndex["Value"] == 4) {
								getUser['Turn'] = 3;
							}
							if (cardIndex["Value"] == 5) {
								let targetUSer = client.findOne({"loginUser.username": cardIndex["User Target"]}).exec()
								.then((getTarget) => {
									let targetCard = getTarget.loginUser["played"].splice(getTarget.loginUser["played"].indexOf(cardIndex["Card Target Value"]), 1);
									let tmpPayload = {};
									tmpPayload["Target Pile"] = cardIndex["Card Target Value"];
									tmpPayload["Pile"] = cardIndex["Card Target Value"];
									tmpPayload["Target Card Index"] = cardIndex["Card Target Index"];
									tmpPayload["Target User"] = cardIndex["User Target"];
//									console.log(tmpPayload);
									socket.in(getRoom).emit('Action', {"Action": "Discard", "Username": userName, "Payload": tmpPayload});	
									socket.in(getRoom).emit('Action', {"Action": "Organize Hand", "Username": userName, "Payload": tmpPayload});
								});
							}
							if (cardIndex["Value"] == 9) {
								Payload["Special Action"] = "Loyalty Card";
								Payload["Target Pile"] = cardIndex["Card Target Value"];
								Payload["Target Card Index"] = cardIndex["Card Target Index"];
							} 
							if (cardIndex["Value"] == 15){
								Payload["Special Action"] = "To-Go Cup";
//								console.log(Payload);
							}
							if (getUser.loginUser['lastPlayed'] == null) getUser.loginUser['lastPlayed'] = params['Card Action'];
							getUser.loginUser['played'].push(getUser.loginUser['hand'][cardIndex["Index"]]);
							Payload["Card Value"] = getUser.loginUser['hand'][cardIndex["Index"]];
							Payload["Card Index"] = cardIndex["Index"];
							Payload["User Target"] = cardIndex['User Target'];
//							console.log(Payload);
//							console.log("- Index: " + cardIndex["Index"] + " [" + getUser.loginUser['hand'][cardIndex["Index"]] + "]");
							socket.in(getRoom).emit('Action', {"Action": "Played", "Username": userName, "Payload": Payload});	
//							if (params['Card Type'] == "Drink Carrier") getUser['Turn'] = params['Turn'];

							
						})
						await resolve(getUser);
					});
				})
				.then(async (getUser) => {
					playList.sort(function(a, b){return b['Index']-a['Index']});
					playList.forEach(async (cardIndex) => {
						let tmpCard = getUser.loginUser['hand'].splice(cardIndex["Index"], 1)[0];
//						console.log("--- " + cardIndex["Index"] + " - " + tmpCard);
					});
					Payload = {};
					Payload["Turns"] = getUser['Turn'];
					Payload["Pile"] = "Hand";
					Payload["Target User"] = userName;
					socket.in(getRoom).emit('Action', {"Action": "Organize Hand", "Username": userName, "Payload": Payload});
//					console.log(" (After)");
//					console.log(" Hand: " + getUser.loginUser["hand"]);
//					console.log(" Played: " + getUser.loginUser["played"]);
					return await getUser.save();
				}).then(async (getUser) => {
					controller.finishAction(userName, getRoom, socket);
				});*/
			} else if (getAction == "Discard Card"){
				tableController.discardCards(socket, getSocket, userName, getRoom, params["Play List"]);
/*				let Payload = {};
				let playList = JSON.parse(params["Play List"]);
//				console.log(playList);
				new Promise(async (resolve, reject) => {
				//await playList.forEach(async (cardIndex) => {
					for(let cardIndex of playList){
					//playList.forEach(async (cardIndex) => {
						await client.findOne({"loginUser.username": cardIndex["User Target"]}).exec()
						.then(async (getTarget) => {
//							console.log(getTarget);
							var discardCard = null;;
							Payload["Target User"] = cardIndex['User Target'];
							Payload["Target Pile"] = (cardIndex['State'] == "Hand" ? "Hand" : cardIndex['Target Value']);
							Payload["Target Index"] = cardIndex['Card Target Index'];
							Payload["Turns"] = getTarget["Turn"];
							socket.in(getRoom).emit('Action', {"Action": "Discard", "Username": userName, "Payload": Payload});
							//await getTarget.save();
						})
					};
					resolve("Done");
				})
				.then(async (done) => {
					return await new Promise(async (resolve, reject) => {
						playList.sort(function(a, b){return b['Index']-a['Index']});
						for(let cardIndex of playList){
					//		console.log(cardIndex);
							let getClient = await client.findOne({"loginUser.username": cardIndex["User Target"]}).exec();
							let getTable = await table.findOne({"tableID": getRoom}).exec();
							await Promise.all([getClient, getTable])
							.then(async (ret) => {
								if (cardIndex["State"] == "Hand") discardCard = ret[0].loginUser['hand'].splice(cardIndex['Card Target Index'], 1)[0];
								else discardCard = ret[0].loginUser[cardIndex['played']].splice(cardIndex['Card Target Value'], 1)[0];
								Payload["Target User"] = cardIndex["User Target"];
								Payload["Pile"] = (cardIndex['State'] == "Hand" ? "Hand" : cardIndex['Card Target Value']);
								ret[1]['discard'].push(discardCard);
								socket.in(getRoom).emit('Action', {"Action": "Organize Hand", "Username": userName, "Payload": Payload});
								await ret[1].save();
								await ret[0].save();
							});
						};
						resolve("Done");
					});
				})
				.then(async (resolve) =>{
					await controller.finishAction(userName, getRoom, socket);
				});
/*				await client.findOne({"loginUser.username": cardIndex["User Target"]}).exec()
				.then(async (getTarget) => {
					await table.findOne({"tableID": getRoom}).exec()
					.then(async (getTable) => {
							Payload["Target User"] = cardIndex['User Target'];
							Payload["Pile"] = (cardIndex['State'] == "Hand" ? "Hand" : cardIndex['Target Value']);
							getTable['discard'].push(discardCard);
							socket.in(getRoom).emit('Action', {"Action": "Organize Hand", "Username": userName, "Payload": Payload});
							return getTable.save();
						});
					});
				}*/
			} else if (getAction == "Finish Action"){
				client.findOne({"loginUser.username": userName}).exec()
				.then(async (getClient) => {
					getClient['Turn'] = getClient['Turn'] - 1;
				});	
			} else if (getAction == "Finishing Turn") {
				socket.in(getRoom).emit('Action', {"Action": "Finish Turn", "Username": userName});	
			} else if (getAction == "Organize Hand") {
				console.log(params);
				client.findOne({"userID": getSocket.id}).exec()
				.then(async (getClient) => {
//					console.log(getClient);
					params["Turns"] = getClient['Turn'];
					params["Target User"] = getClient["username"];
					return await getClient.save();	
				})
				.then(async (getClient) => {
					socket.in(getRoom).emit('Action', {"Action": "Organize Hand", "Username": userName, "Payload": params});
				});
			} else if (getAction == "Target Card") {
				socket.in(getRoom).emit('Action', {"Action": "Target Card", "Username": params["User Target"], "Index": params["Card Index"], "Value": params["Card Pile"], "Message": params["Card Action"]});
			} else if (getAction == "Game Over") {				
				socket.in(getRoom).emit('Action', {"Action": "Game Over", "Username": userName});
			} else if (getAction == "Leave Table"){
				//console.log(userName + " Leave Table");
				return await controller.emptyTableCheck(userName, getRoom, socket)
				.then(async () => {
					return await user.findOne({"username": userName}).exec();
				})
				.then(async (getUser) => {
					let resetStatus = await controller.resetClient(getSocket.id, getUser, getSocket); 
					getSocket.join("Main Lobby");
					getSocket.leave(getRoom);
					return resetStatus
				});
			} else if (getAction == "Resolve Table"){
				//console.log(userName + " Resolve Table");
				let getClient = await client.findOne({"loginUser.username": userName}).exec()
				.then(async (getClient) => {
					getClient["PurpleCount"] = params["Purple Count"];
					getClient["Score"] = params["Score"];
					getClient["State"] = "Resolved";
					return getClient.save();
				});	

				await client.countDocuments({"tableID": getRoom, "State": {$ne: "Resolved"}}).exec()
				.then(async (getCount) => {
					if (getCount == 0){
						return await client.find({"tableID": getRoom})
						.sort('-PurpleCount')
						.exec();
					} else {
						return null;
					}
				})
				.then(async (clientLists) => {

					//console.log(clientLists);
					if (clientLists != null){
						if (clientLists.length > 1){
							if (clientLists[0]["Score"] != clientLists[1]["Score"]) clientLists[0]["Score"] += 12;
						} else 
							clientLists[0]["Score"] += 12;
					}
					
					clientLists.sort(function(a, b){return b["Score"]-a["Score"]});
					//console.log(clientLists);
				});
			}
		});	
	});
};
