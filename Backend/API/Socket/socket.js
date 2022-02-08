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
			let payload = {};			
			switch(param["Action"]){
				case "Register":
					await user.findOne({"username": param['Username']}).exec()
					.then(async (get_login) => {
						if (get_login != null){
							console.log(getSocket.id + ' Denied connected as ' + param['Username'] + 'Reason: user exists');
							payload["Action"] = "Denied";
							payload["Reason"] = "User exists";

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
					getSocket.join(param['lobbyID']);
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
		});
	
	  	getSocket.on('disconnect', async () => {
//			console.log("User Disconnected");
		});

		getSocket.on("Reply", async (payload) => {
			let param = JSON.parse(payload);
			let data = JSON.parse(param["data"]);
			console.log(param);
			socket.to(param["target"]).emit('Reply', payload.replace(/"/g, "`"))

		})

		getSocket.on("Broadcast", async (payload) => {
			let param = JSON.parse(payload);
			socket.in(param['lobbyID']).emit('Broadcast', payload.replace(/"/g, "`"));
		});

		getSocket.on("Other", async (payload) => {
			let param = JSON.parse(payload);
			getSocket.to(param['lobbyID']).emit('Broadcast', payload.replace(/"/g, "`"));			
		})

		getSocket.on("Server", async (payload) => {
			let param = JSON.parse(payload);
			let data = JSON.parse(param["data"])

			switch(data["Action"]){
				case "Enter Game":
					let payload_template = {};
					payload_template["source"] = "";
					payload_template["data"] = "";
					payload_template["target"] = "";

//					console.log(socket.sockets.adapter.rooms);
					for(let resourceID = 0; resourceID < 2; resourceID++){
						let resourcePayloads = {};
						resourcePayloads["Resource"] = (resourceID == 0) ? "Tree" : "Stone";

						for(let counter = 0; counter < 25; counter++){
							resourcePayloads["xPos"] = Math.floor(Math.random() * 1001) - 500
							resourcePayloads["yPos"] = Math.floor(Math.random() * 1001) - 500				
							resourcePayloads["UID"] = randomIDGen(12);
							resourcePayloads["Action"] = "Spawn Resource";
							resourcePayloads["Type"] = "Action";
							payload_template["data"] = resourcePayloads;
							socket.in(data['lobbyID']).emit('Broadcast', JSON.stringify(payload_template).replace(/"/g, "`"));
						}
					}

					let payloadData = {};
					payloadData["Action"] = "Resource Loaded";
					payloadData["Type"] = "Action";
					payload_template["data"] = payloadData;
					socket.in(data['lobbyID']).emit('Broadcast', JSON.stringify(payload_template).replace(/"/g, "`"));
					break;
				case "Create Lobby":
					let new_room = "Lobby_" + randomIDGen(12);
					getSocket.join(new_room);
					break;				
			}
		})
	});
};

async function saveToFile(in_name, in_data){
	return new Promise(async (resolve, reject) => {
		fs.writeFile('/var/www/html/Json/' + in_name + ".json", in_data, err => {
			if (err){
				console.error("Error writing: " + err)
			}
		})
		resolve(in_data);
	})
}

function randomIDGen(length){
	var result = "";
	var chars = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
	for (var i = 0; i < length; i++){
		result += chars.charAt(Math.floor(Math.random() * chars.length));
	}
	return result;
}