const models = require('../Model/gameModel'),
	mongoose = require('mongoose'),
	lobby = mongoose.model('Lobby'),
	resource = mongoose.model('Resources'),
	player = mongoose.model('Player'),
	user = mongoose.model('User'),
	item = mongoose.model('Item'),
	fs = require('fs'),
	currentTime = Date.now();

module.exports = async function(socket){
	socket.on('connect', async (getSocket) => {
		var message = {}

		getSocket.on('Login', async (getPayload) => {
			let param = JSON.parse(getPayload);
//			if (param['action'] == "Create"){
//				new lobby({"lobbyID": randomIDGen(5), "hostID": param['Username']}).save();
//			}
			console.log(getPayload);
			let payload = {};
			

			switch(param["Action"]){
				case "Register":
					await user.findOne({"username": param['Username']}).exec()
					.then(async (get_login) => {
						if (get_login != null){
							console.log(getSocket.id + ' Denied connected as ' + param['Username'] + 'Reason: user exists');
							payload["Action"] = "Denied";
							payload["Reason"] = "User exists";
							// await lobby.find({}).exec()
							// .then(async (getLobby) => {
							// 	payload["Server List"] = getLobbies;
							// 	return
							// })
						} 
						else {							
							payload["Action"] = "Welcome";
							new user({"username": param['Username'], "password": param["Password"]}).save();
							await lobby.findOne({}).exec()
							.then(async (getLobby) => {
								console.log(getLobby);
								if (getLobby == null){
									getLobby = await new lobby({"lobbyID": randomIDGen(5), "resourceLimit": 75, "hostID": param['Username']}).save();
								}
								payload["Server"] = getLobby['lobbyID'];
								payload['Username'] = param['Username'];
								payload['UserID'] = getSocket.id;
							})
						}
					})
					getSocket.emit("Action", payload);
					break;				
				case "Login":
					await user.findOne({"username": param['Username'], "password": param['Password']}).exec()
					.then(async (get_login) => {
						if (get_login != null){
							payload["Action"] = "Welcome";
							await lobby.findOne({}).exec()
							.then(async (getLobby) => {
								if (getLobby == null){
									getLobby = await new lobby({"lobbyID": randomIDGen(5), "resourceLimit": 75, "hostID": param['Username']}).save();
								}
								payload["Server"] = getLobby['lobbyID'];
								payload['Username'] = param['Username'];
								payload['UserID'] = getSocket.id;
							})
						} 
						else {
							console.log(getSocket.id + ' Denied connected as ' + param['Username'] + 'Reason: Bad Password');
							payload["Action"] = "Denied";
							payload["Reason"] = "Invalid credentials";
						}
					})
					getSocket.emit("Action", payload);
					break;
				case "Join":
					await lobby.findOne({"lobbyID": param['lobbyID']}).exec()
					.then(async (getLobby) => {
						let survivorTeam = player.countDocuments({"lobbyID": getLobby['lobbyID'], "Team": "Survivor"})
						let virusTeam = player.countDocuments({"lobbyID": getLobby['lobbyID'], "Team": "Virus"})
						return await Promise.all([getLobby, survivorTeam, virusTeam])
					})
					.then(async (result) => {
						player.findOne({"name": param['Username']}).exec()
						.then(async (findPlayer) =>{
							if (findPlayer == null){
								payload['Username'] = param['Username'];
								payload['UserID'] = getSocket.id;
								payload['lobbyID'] = result[0]['lobbyID'];
								getSocket.nickname = param['Username'];
								payload['Team'] = result[2] <= 0 ? "Virus" : "Survivor"; 
								//console.log(getSocket.id + ' Success  connected as ' + param['Username']);
								// console.log(result[0])
								// console.log(param['Username'])
								if (result[0]['hostID'] == param['Username'])
								{
									new player({"UserID": getSocket.id, "health": 100, "lobbyID": result[0]['lobbyID'], "Team": payload['Team'], "host": true, "name": param['Username']}).save();
								}
								else 
								{
									player.findOne({"lobbyID": result[0]['lobbyID'], "host": true}).exec()
									.then(async (findHost) => {
										// console.log("Host:" + JSON.stringify(findHost));
										if (findHost == null)
											new player({"UserID": getSocket.id, "health": 100, "lobbyID": result[0]['lobbyID'], "Team": payload['Team'], "host": true, "name": param['Username']}).save();
										else 
											new player({"UserID": getSocket.id, "health": 100, "lobbyID": result[0]['lobbyID'], "Team": payload['Team'], "host": false, "name": param['Username']}).save();
									});
								}
								payload["Action"] = "Enter Game";
								getSocket.emit("Action", payload);
							} else {
								console.log(getSocket.id + ' Denied connected as ' + param['Username']);
								payload["Action"] = "Denied";
								getSocket.emit("Action", payload);
							}
						});
					})
					break;
			}
		});

// 		setInterval(function(){
// //			player.find({"UserID": {$ne : getSocket.id}}).exec()
// 			player.find({}).exec()
// 			.then(async (getTeam) => {
// 				getSocket.emit('Mass Update', getTeam);
// 			});
// 		}, 30);

		getSocket.on('disconnecting', async () => {

			player.findOne({"UserID": getSocket.id}).exec()
			.then(async (getPlayer) => {
				if (getPlayer != null){
					await saveToFile(getPlayer["name"], getPlayer)
					.then(async (getPlayer) => {
						await player.deleteOne({"UserID": getSocket.id});
					})
				}
			})
				return 
			// .then(async (getPlayer) => {
			// 	return player.findOne({"host": true, "lobbyID": getPlayer['lobbyID']}).exec();
			// })
			// .then(async (getHost) => {
			// 	console.log(getHost);
			// });
//			return await player.deleteOne({"UserID": getSocket.id})
/*			.then(async () => {
				return player.find({}).exec()
			})
			.then(async (getPlayers) => {
				socket.emit("Lobby Update", getPlayers);
			}); */
		});
	
	  	getSocket.on('disconnect', async () => {
			console.log("User Disconnected");
		});


		getSocket.on("Lobby", async (payload) => {
			let param = JSON.parse(payload);
			let newPayLoad = {};
			switch(param["Action"]){
				case "Start Game":
					player.findOne({"UserID": getSocket.id}).exec()
					.then(async (getPlayer) => {
						getPlayer["State"] = "Ready";
						return await getPlayer.save();
					})
					.then(async () => {
						return await player.find({"State": {$ne : "Ready"}}).exec()
					})
					.then(async (getPlayers) => {
						if (getPlayers.length == 0){
							newPayLoad["Action"] = "Begin Game";
							socket.emit("Lobby", newPayLoad);
						} else {
							newPayLoad["Action"] = "Recieve";
							newPayLoad["Username"] = "Server";
							newPayLoad["Message"] = "Waiting for " + getPlayers.length + " more to be ready";
							socket.emit("Lobby", newPayLoad);
						}
					})
				break;
				case "Refresh Userlist":
					player.find({}).exec()
					.then(async (getPlayers) => {

						sendPacket(socket, "Update Room List", getPlayers)
						// newPayLoad["Action"] = "Update Room List";
						// newPayLoad["Message"] = ;
						// socket.emit('Lobby Update', getPlayers);
					});
				break;
			}	
		});


		getSocket.on("Update" , async (payload) => {
			let param = JSON.parse(payload);
			param['userID'] = getSocket.id;
			socket.emit('Update', param);
//			getSocket.broadcast.emit('Update',  param);
			// player.findOne({"name": param['Username']}).exec()
			// .then(async (getPlayer) => {
			// 	getPlayer['xPos'] = param['xPos'];
			// 	getPlayer['yPos'] = param['yPos'];
			// 	getPlayer['zPos'] = param['zPos'];
			// 	getPlayer['xRot'] = param['xRot'];
			// 	getPlayer['yRot'] = param['yRot'];
			// 	getPlayer['zRot'] = param['zRot'];
			// 	getPlayer['Team'] = param['Team'];
			// 	getPlayer['State'] = param['State'];
			// 	getPlayer['WeaponState'] = param['WeaponState'];
			// 	await getPlayer.save();
			// })
//			await player.updateOne({"userID": getSocket.id}, {xPos: param["xPos"], yPos: param["yPos"], zPos: param["zPos"]});
		});

		
		getSocket.on("Enter Game", async () => {
			let param = {};
			param["Action"] = "Enter";

			player.findOne({"UserID": getSocket.id}).exec()
			.then(async (getPlayer) => {
				getPlayer["State"] = "Alive";
				return await getPlayer.save();
			})
			.then(async () => {
				return await player.countDocuments({"State": "Ready"}).exec()
			})
			.then(async (getPlayers) => {
				//console.log(getPlayers);
				if (getPlayers == 0){
					console.log(getSocket.id);
					await player.find({"State": "Alive"}).exec()
					.then(async (getAllAlive) => {
					        for (let perPlayer of getAllAlive){
							await new Promise(async (resolve, reject) => {
								param["UserID"] = perPlayer['UserID']
								param["Username"] = perPlayer['name']
								socket.emit("Action", param);
								resolve("Done");
							});
						}
					});
				}
			})
		});


		getSocket.on("Message", async (payload) => {
			let param = JSON.parse(payload);
			param['UserID'] = getSocket.id;
			param["Username"] = getSocket.nickname;
			param["Action"] = "Recieve";
			socket.emit("Lobby", param);			
		});

		getSocket.on("Everyone", async (payload) => {
			let param = JSON.parse(payload);
			socket.emit('Action', param);
/*			let paramSpec = await specialCases(param, socket);
			console.log(paramSpec);
			if (paramSpec != null){
				console.log(paramSpec);
				socket.emit('Action', paramSpec);
			} else {
//			param['UserID'] = getSocket.id;
				param['Username'] = getSocket.nickname;
				socket.emit('Action', param);
			}*/
		});

		getSocket.on("Broadcast", async (payload) => {
			let param = JSON.parse(payload);
			console.log(param);
			socket.emit('Broadcast', payload.replace(/"/g, "`"));
		});

		getSocket.on("Self", async (payload) => {
			let param = JSON.parse(payload);
			selfResponse(param, getSocket);
		});

		getSocket.on("Server", async (payload) => {
			let param = JSON.parse(payload);
			serverResponse(param, socket, getSocket);
		});

		getSocket.on("Others", async (payload) => {
			let param = JSON.parse(payload);
			param['UserID'] = getSocket.id;
			getSocket.broadcast.emit('Action', param);
		});

		getSocket.on("Minion", async (payload) => {
			let param = JSON.parse(payload);
			param['UserID'] = getSocket.id;
			getSocket.broadcast.emit('Minion',  param);
		});

/*		getSocket.on("Register", async (getAction, getUsername, getPassword) => {
			switch(getAction){
				case "Check":
					await loginController.registerCheckClient(socket, getSocket, getUsername, getPassword);
					break;
				case "Validate":
					await loginController.registerValidateClient(socket, getSocket, getUsername, getPassword);
					break;
			};
		});


		getSocket.on("Login", async (getAction, getUsername, getPassword) => {
			switch(getAction){
				case "Check":
					await loginController.loginCheckClient(socket, getSocket, getUsername, getPassword);
					break;
				case "Validate":
					await loginController.loginValidateClient(socket, getSocket, getUsername, getPassword);
					break;
				case "Log In":
					await loginController.loginClient(socket, getSocket, getUsername, getPassword);
					break;
			};		
		});

		getSocket.on("Lobby", async (getAction, getUsername, getTableID, getParams) => {
			let params = JSON.parse(getParams);
			switch(getAction){
				case "Create":
					lobbyController.createTable(socket, getSocket, getUsername, getTableID);
					break;
				case "Join":
					lobbyController.joinTable(socket, getSocket, getUsername, getTableID);	
					break;
				case "Begin":
					lobbyController.beginTable(socket, getSocket, getUsername, getTableID);
					break;
				case "Refresh Tables":
					lobbyController.refreshTableList(socket, getSocket, getUsername, getTableID);
					break;
			};
		});

		getSocket.on("Game", async (getAction, getUsername, getTableID, getParams) => {
//			console.log(getAction);
			let params = JSON.parse(getParams);
			let Payload = {};
//			console.log(getParams);
			switch(getAction){
				case "Game Entered":
					tableController.enterGame(socket, getSocket, getUsername, getTableID);
					break;
				case "Prepare Table":
					tableController.prepareTable(socket, getSocket, getUsername, getTableID);
					break;
				case "Get Suggestion":
					tableController.getSuggestion(socket, getSocket, getUsername, getTableID);
					break;
				case "Play Card":
					tableController.playCard(socket, getSocket, getUsername, getTableID, params["Play List"]);
					break;
				case "Discard Card":
					tableController.discardCards(socket, getSocket, getUsername, getTableID, params["Play List"]);
					break;
				case "Game Over":
					tableController.gameOver(socket, getSocket, getTableID);
					break;
				case "Leave Table":
					tableController.leaveTable(socket, getSocket, getUsername, getTableID);
					break;
				case "Resolve Table":
					tableController.resolveTable(socket, getSocket, getUsername, getTableID);
					break;
			}
		});	*/
	});
};

async function selfResponse(param, socketType)
{
	let payload = {};
	switch(param['Action'])
	{
		case "Join Game":
			let getPlayer = player.findOne({"name": param['Username']});
			let getLobby = lobby.findOne({"lobbyID": param['lobbyID']});

			Promise.all([getPlayer, getLobby])
			.then(async (result) => {
//				console.log(result);
				res = JSON.parse(JSON.stringify(result[0]));
				res['Action'] = "Join Game";
				res['resourceLimit'] = result[1]["resourceLimit"];		
				res["Time"] = Math.floor((Date.now() - currentTime)/1000);
				socketType.emit('Action', res);
				});
			break;
		case "Server Update":
                        return await resource.find({"lobbyID": param["lobbyID"]}).exec()
                        .then(async (getAllResource) => {
				for (let resourceIndex of getAllResource){
					res = JSON.parse(JSON.stringify(resourceIndex));
					res['Action'] = "Spawn Resource";		
					socketType.emit('Resource Update', res);
				}
				return await item.find({"lobbyID": param["lobbyID"]}).exec()
                        })
			.then(async (getAllItem) => {
				for (let itemIndex of getAllItem){
					res = JSON.parse(JSON.stringify(itemIndex));
					res['Action'] = "Spawn Item";		
					socketType.emit('Resource Update', res);				
				}
			});
			break;
	}
}

async function serverResponse(param, everyone, selfSocket)
{
	let payload = {};
	switch(param['Action'])
	{
		case "Death":
			player.findOne({"UserID": getSocket.id}).exec()
			.then(async (getPlayer) => {
				getPlayer['State'] = "Dead";
				await getPlayer.save();
			});
			break;
		case "Spawn Resource":
                        res = JSON.parse(JSON.stringify(param));
                        res['Action'] = "Spawn Resource";
			res['UID'] = randomIDGen(10);
			new resource(res).save();
			everyone.emit('Action', res);
			break;
		case "Debug Time":
			payload = {};
			payload["Action"] = "Debug Time";
			payload["Time"] = Math.floor((Date.now() - currentTime)/1000);
			payload["Username"] = param["Username"];
			selfSocket.emit('Action', payload);
			break;			
		case "Spawn Item":
                        res = JSON.parse(JSON.stringify(param));
                        res['Action'] = "Spawn Item";
			new item(res).save();
			everyone.emit('Action', res);
			break;
		case "Pickup Item":
			console.log(param);
			item.deleteOne({"UID": param["UID"]}).exec();
			break;	
		case "Damage Resource":
			console.log("Damage Resource");
			resource.findOne({"UID": param["UID"]}).exec()
			.then(async (getResource) => {
				getResource['durability'] += Number(param['damage']);
				return await getResource.save();
			})
			.then(async (getResource) => {
				res = JSON.parse(JSON.stringify(getResource));
				res['Action'] = "Update Resource";
				everyone.emit('Resource Update', res);
				if (res['durability'] <= 0){
				await resource.deleteOne({"UID": param["UID"]});	
				}
			});
			break;
		case "Damage Living":
			player.findOne({"name": param["Target"]}).exec()
			.then(async (getLiving) => {
				getLiving['health'] += Number(param['damage']);
				if (getLiving['health'] <= 0){
					getLiving['State'] = "Dead";
				}
				return await getLiving.save();
			})
			.then(async (getLiving) => {
				res = JSON.parse(JSON.stringify(getLiving));
				res['Action'] = "Update Living";
				everyone.emit('Resource Update', res);
			});
			break;
	}
}

async function saveToFile(in_name, in_data){
	return new Promise(async (resolve, reject) => {
//		console.log(in_name);
//		console.log(in_data);
		fs.writeFile('/var/www/html/Json/' + in_name + ".json", in_data, err => {
			if (err){
				console.error("Error writing: " + err)
			}
		})
		resolve(in_data);
	})
}

async function specialCases(param, socketType){
	let payload = {};
}

function randomIDGen(length){
	var result = "";
	var chars = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
	for (var i = 0; i < length; i++){
		result += chars.charAt(Math.floor(Math.random() * chars.length));
	}
	return result;
}

async function sendPacket(in_socket, in_action, in_data)
{
	return await new Promise(async (resolve, reject) => {
		let newParam = {};
		newParam["Action"] = in_action;
		newParam["Data"] = JSON.stringify(in_data).replace(/"/g, "`");
		in_socket.emit("Get Data", newParam);
		resolve("Done");
	})
}

async function sendPacket(in_socket, in_type, in_data)
{
	return await new Promise(async (resolve, reject) => {
		let newParam = {};
		newParam["data"] = JSON.stringify(in_data).replace(/"/g, "`");
		in_socket.emit("Get Data", newParam);
		resolve("Done");
	})
}