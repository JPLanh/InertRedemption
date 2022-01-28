'use strict';

const mongoose = require('mongoose'),
        user = mongoose.model("User"),
        client = mongoose.model("Client"),
	card = mongoose.model("Card"),
        table = mongoose.model("Table"),
	userCard = mongoose.model("UserCard");

exports.enterGame = async function(getSocket, getUserSocket, getUsername, getTableID){
	let getTable = await table.findOne({"tableID": getTableID}).exec();
	let readyCount = await client.findOne({"loginUser.username": getUsername}).exec()
        .then(async (getClient) => {
        	getClient["State"] = "Preparing";
                await getClient.save();
        })
        .then(async () => {
                return new Promise((resolve, reject) => {
                        let userList = [];
                	Object.keys(getSocket.sockets.adapter.rooms[getTableID].sockets).forEach((perClient) => {
                        	userList.push(getSocket.sockets.connected[perClient].nickname);
                        });
                        resolve(userList);
                })
        })
	.then(async (listOfClients) => {
                getSocket.in(getTableID).emit('Action', {"Action": "Join", "Username": getUsername, "Room": getTableID, "UserList": listOfClients});
		return listOfClients;
        });

	Promise.all([getTable, readyCount])
	.then(async (ret) => {
		await client.find({"tableID": getTableID, "State": "Preparing"}).exec()
		.then(async (clientsFound) => {
			if (clientsFound.length == ret[1].length) {
				getSocket.emit('Action', {"Action": "Prepare Table", "Username": ret[0]['activeUser']});
			}
		})
	})
}

exports.prepareTable = async function(getSocket, getUserSocket, getUsername, getTableID){

        let fetchClient = await client.findOne({"loginUser.username": getUsername}).exec();
	let getUserCards = await userCard.find({"clientRef.loginUser.username": getUsername}).exec();
        let getTable = await table.findOne({"tableID": getTableID}).exec();

        Promise.all([fetchClient, getTable, getUserCards])
        .then(async (ret) => {
		let payload = {};
		if (ret[2].length < 5){
			new Promise(async (resolve, reject) => {
				resolve(await drawCard(getSocket, getUserSocket, ret[1], ret[0]));
			})
			.then(async () => {
		                let index = ret[1].order.indexOf(ret[0].loginUser['username']);
		               //console.log(getTable);
				ret[1]['activeUser'] = ret[1].order[(index+1)%ret[1].order.length];
		               //console.log(getTable);
		                return await ret[1].save();
			})
			.then(async (fetchTable) => {
				getSocket.in(getTableID).emit('Action', {"Action": "Prepare Table", "Username": fetchTable['activeUser']});
			})
		} else {
			getSocket.in(getTableID).emit("Action", {"Action": "Continue Turn", "Username": ret[1]['activeUser']});
		}
	});
}

exports.organizeHand = async function(getSocket, getUsername){
	let payload = {};
	client.findOne({"loginUser.username": getUsername}).exec()
	.then(async (getClient) => {
		payload["Turns"] = getClient['Turn'];
		payload["Target User"] = getClient["username"];
		getSocket.in(getClient["tableID"]).emit('Action', {"Action": "Organize Hand", "Username": getClient['loginUser.userName'], "Payload": payload});
	})
}

exports.getSuggestion = async function(getSocket, getUserSockket, getUsername, getTableID){
        client.find({"tableID": getTableID}).select('-userID -__v -State -Turn -tableID -_id -loginUser._id').exec()
        .then(async (getClientList) => {
//                controller.cardPossabilities(getClient.loginUser['hand'], []);
        });
        return null;
}

exports.playCard = async function(getSocket, getUserSocket, getUsername, getTableID, getCardList){
	let playList = JSON.parse(getCardList);

	let Payload = {};
	for (let cardIndex of playList){
		let cardFetch = await userCard.findById(cardIndex["_id"]);
		let userFetch = await client.findOne({"loginUser.username": cardIndex['User Target']});

		await Promise.all([cardFetch, userFetch])
		.then(async (ret) => {

			let score = await playCardHelper(getSocket, getUserSocket, ret[1], ret[0]);
			let payload = {};
                        payload["Target User"] = cardIndex['User Target']
			payload["Card ID"] = ret[0]["_id"];
			payload["name"] = ret[0].cardRef['name'];
			payload["category"] = ret[0].cardRef['category'];
			payload["Score"] = score
                        getSocket.in(getTableID).emit('Action', {"Action": "Played", "Username": getUsername, "Payload": payload});
		});
	}
        Payload["Pile"] = "Hand";
	Payload["Target User"] = getUsername;
	getSocket.in(getTableID).emit('Action', {"Action": "Organize Hand", "Username": getUsername, "Payload": Payload});
        return await client.findOne({"loginUser.username": getUsername}).exec()
        .then(async (fetchUser) => {
		await actionRegulator(getSocket, getUserSocket, fetchUser);
	});
/*
        return await client.findOne({"loginUser.username": getUsername}).exec()
        .then(async (getUser) => {
//		console.log(getUser);
        	return new Promise(async (resolve, reject) => {
	                let Payload = {};
        	        Payload["Special Action"] = "None";
        	        playList.forEach(async (cardIndex) => {
        	        	if (cardIndex["Value"] == 4) {
	        	        	getUser['Turn'] = 3;
	        	        }
	        	        if (cardIndex["Value"] == 5) {
	        	        	client.findOne({"loginUser.username": cardIndex["User Target"]}).exec()
	        	        	.then(async (getTarget) => {
	        	                	let targetCard = getTarget.loginUser["played"].splice(getTarget.loginUser["played"].indexOf(cardIndex["Card Target Value"]), 1);
	        	                        let tmpPayload = {};
	        	                        tmpPayload["Target Pile"] = cardIndex["Card Target Value"];
	        	                        tmpPayload["Pile"] = cardIndex["Card Target Value"];
	        	                        tmpPayload["Target Card Index"] = cardIndex["Card Target Index"];
	        	                        tmpPayload["Target User"] = cardIndex["User Target"];
	//      	                        console.log(tmpPayload);
	        	                        getSocket.in(getTarget['tableID']).emit('Action', {"Action": "Discard", "Username": getUsername, "Payload": tmpPayload});
	        	                        getSocket.in(getTarget['tableID']).emit('Action', {"Action": "Organize Hand", "Username": getUsername, "Payload": tmpPayload});
	        	                });
	        	        }
	        	        if (cardIndex["Value"] == 9) {
	        	                Payload["Special Action"] = "Loyalty Card";
	        	                Payload["Target Pile"] = cardIndex["Card Target Value"];
	                	        Payload["Target Card Index"] = cardIndex["Card Target Index"];
	                	}
	                	if (cardIndex["Value"] == 15){
	                	        Payload["Special Action"] = "To-Go Cup";
	                	}
//	                	if (getUser.loginUser['lastPlayed'] == null) getUser.loginUser['lastPlayed'] = params['Card Action'];
	                	getUser.loginUser['played'].push(getUser.loginUser['hand'][cardIndex["Index"]]);
	                	Payload["Card Value"] = getUser.loginUser['hand'][cardIndex["Index"]];
	                	Payload["Card Index"] = cardIndex["Index"];
	                	Payload["User Target"] = cardIndex['User Target'];
//		              	console.log(Payload);
	                	getSocket.in(getUser['tableID']).emit('Action', {"Action": "Played", "Username": getUsername, "Payload": Payload});    
	        	})
	        	await resolve(getUser);
		});
	})
        .then(async (getUser) => {
//		console.log(getUser);
                playList.sort(function(a, b){return b['Index']-a['Index']});
                playList.forEach(async (cardIndex) => {
                	let tmpCard = getUser.loginUser['hand'].splice(cardIndex["Index"], 1)[0];
                });
                let Payload = {};
                Payload["Turns"] = getUser['Turn'];
                Payload["Pile"] = "Hand";
                Payload["Target User"] = getUsername;
                getSocket.in(getUser['tableID']).emit('Action', {"Action": "Organize Hand", "Username": getUser['loginUser.username'], "Payload": Payload});
                return await getUser.save();
        }).then(async (getUser) => {
                await actionRegulator(getSocket, getUserSocket, getUser);
        });
*/	
}

async function playCardHelper(getSocket, getUserSocket, getClient, getCard){
//        console.log("Before User: " + getClient.loginUser['username'] + " , Card: " + getCard.cardRef['name'] + " , Score: " + getClient['score']);
//	console.log(getCard);
        switch(getCard.cardRef['category']){
		case "Action":
			switch(getCard.cardRef['name'])
			{
				case "Complimentary Cookie":
					break;
				case "Drinks Carrier":
					break;
				case "Drink Spill":
					break;
				case "Loyalty Card":
					break;
				case "To-Go Cup":
					break;
			}
			break;
                case "Blue":
                        getCard['state'] = "Played";
			getCard['clientRef'] = getClient;
                        await getCard.save()
                        .then(async (getCard) => {
                                return await userCard.countDocuments({"clientRef.loginUser.username": getClient.loginUser['username'], "tableRef.tableID": getClient['tableID'], "state": "Played", "cardRef.category": "Blue"}).exec()                        
                        })
                        .then(async (fetchCards) => {
                                if (fetchCards > 0 && fetchCards%2 == 0) getClient['score'] += 5;
                         });
                        break;
                case "Yellow":
                        getCard['state'] = "Played";
			getCard['clientRef'] = getClient;
                        await getCard.save()
                        .then(async (getCard) => {
                                return await userCard.countDocuments({"clientRef.loginUser.username": getClient.loginUser['username'], "tableRef.tableID": getClient['tableID'], "state": "Played", "cardRef.category": "Yellow"}).exec()                        
                        })
                        .then(async (fetchCards) => {
                                if (fetchCards > 0 && fetchCards%3 == 0) getClient['score'] += 12;
                         });
                        break;
                case "Purple":
                        let beforeCount = await userCard.aggregate()
                        .match({"tableRef.tableID": getClient['tableID'], "state": "Played", "cardRef.category": "Purple" })
                        .group({_id: "$clientRef.loginUser.username", count: { $sum: 1 }}).exec();

                        getCard['state'] = "Played";
			getCard['clientRef'] = getClient;
                        await getCard.save()

                        if (beforeCount.length == 0)
                        {
                                getClient['score'] += 15;

                        } else if (beforeCount.length == 1) 
			{
				if (beforeCount[0]._id != getClient.loginUser['username']){
					await client.findOne({'loginUser.username': beforeCount[0]._id}).exec()
                                        .then(async (fetchClient) => {
                                        	fetchClient['score'] -= 15;
                                                let payload = {};
                                                payload['Score'] = fetchClient['score'];
                                                getSocket.in(fetchClient['tableID']).emit('Action', {"Action": "Update Score", "Username": beforeCount[0]._id, "Payload": payload});
       	                                        return await fetchClient.save();
       	                       		});
				}
                        } else 
			{
                                await userCard.aggregate()
                                .match({"tableRef.tableID": getClient['tableID'], "state": "Played", "cardRef.category": "Purple" })
                                .group({_id: "$clientRef.loginUser.username", count: { $sum: 1 }}).exec()
                                .then(async (afterCount) => {
					console.log("=========");
					console.log(beforeCount);
					console.log(afterCount);
					if (afterCount.length > 1){
                                        	if (beforeCount[0].count == beforeCount[1].count){
                                                	if (afterCount[0]._id == getClient.loginUser['username']){
                                                        	getClient['score'] += 15;                                                
                                                	}
                                        	} else {
							if (afterCount[0].count == afterCount[1].count){
                                                		await client.findOne({'loginUser.username': beforeCount[0]._id}).exec()
                                                		.then(async (fetchClient) => {
                                                        		fetchClient['score'] -= 15;
                                                        		let payload = {};
                                                        		payload['Score'] = fetchClient['score'];
                                                        		getSocket.in(fetchClient['tableID']).emit('Action', {"Action": "Update Score", "Username": beforeCount[0]._id, "Payload": payload});
       	                                                		return await fetchClient.save();
       	                           	             		});
							}
	                                        }
					}
                                });
                        }
                        break;
                case "Brown":
                        getCard['state'] = "Played";
			getCard['clientRef'] = getClient;
                        await getCard.save()
                        .then(async (getCard) => {
                                return await userCard.countDocuments({"clientRef.loginUser.username": getClient.loginUser['username'], "tableRef.tableID": getClient['tableID'], "state": "Played", "cardRef.category": "Brown"}).exec()                        
                        })
                        .then(async (fetchCards) => {
                                if (fetchCards == 1) getClient['score'] += 12;
                                if (fetchCards == 2) getClient['score'] -= 12;
                         });
                        break;
                default:                
                        getCard['state'] = "Played";
			getCard['clientRef'] = getClient;
                        switch(getCard.cardRef['name']){
                                case "Hot Chocolate":
                                        getClient['score'] -= 2;
                                        break;
                                case "Fruit Tea":
                                        getClient['score'] -= 3;
                                        break;
                                case "Roobios":
                                        getClient['score'] -= 4;
                                        break;
                                case "Earl Grey":
                                        getClient['score'] += 2;
                                        break;
                                case "Green Tea":
                                        getClient['score'] += 3;
                                        break;
                                case "English Breakfast":
                                        getClient['score'] += 4;
                                        break;
                        }
                        await getCard.save()
                break;
        }
        await getClient.save();
//        console.log("After User: " + getClient.loginUser['username'] + " , Card: " + getCard.cardRef['name'] + " , Score: " + getClient['score']);
//	console.log(getCard);
        return getClient['score'];

}


exports.discardCards = async function(getSocket, getUserSocket, getUsername, getTableID, getCardList) {
	let Payload = {};
        let playList = JSON.parse(getCardList);
//      console.log(playList);

	for (let cardIndex of playList){
		await userCard.findById(cardIndex["_id"])
		.then(async (fetchUserCard) => {
			let payload = {};
			fetchUserCard["state"] = "Discard";
                        payload["Target User"] = getUsername
			payload["Card ID"] = fetchUserCard["_id"];
                        getSocket.in(getTableID).emit('Action', {"Action": "Discard", "Username": getUsername, "Payload": payload});
			await fetchUserCard.save();
		})
	}
        Payload["Pile"] = "Hand";
	Payload["Target User"] = getUsername;
	getSocket.in(getTableID).emit('Action', {"Action": "Organize Hand", "Username": getUsername, "Payload": Payload});

        return await client.findOne({"loginUser.username": getUsername}).exec()
        .then(async (fetchUser) => {
		await actionRegulator(getSocket, getUserSocket, fetchUser);
	});
/*
	return userCard.find({"clientRef.loginUser.username": getUsername}).exec()
	.then(async (fetchUserCards) => {
		console.log(fetchUserCards);
		for(let fetchCard of fetchUserCards){
			let payload = {};
			fetchCard["clientRef"] = null;
			fetchCard["state"] = "Discard";
                        payload["Target User"] = getUsername
			payload["Card ID"] = fetchCard["_id"];
                        getSocket.in(getTargetID).emit('Action', {"Action": "Discard", "Username": getUsername, "Payload": payload});
			await fetchCard.save();
		}
                getSocket.in(getTableID).emit('Action', {"Action": "Organize Hand", "Username": getUsername, "Payload": Payload});
		return await client.findOne({"loginUser.username": getUsername}).exec();
	})
        .then(async () => {
                return await new Promise(async (resolve, reject) => {
        	        playList.sort(function(a, b){return b['Index']-a['Index']});
                        for(let cardIndex of playList){
//              		console.log(cardIndex);
                                let getClient = await client.findOne({"loginUser.username": cardIndex["User Target"]}).exec();
                                let getTable = await table.findOne({"tableID": getTableID}).exec();
                                await Promise.all([getClient, getTable])
                                .then(async (ret) => {
					let discardCard = "";
                                	if (cardIndex["State"] == "Hand") discardCard = ret[0].loginUser['hand'].splice(cardIndex['Card Target Index'], 1)[0];
                                        else discardCard = ret[0].loginUser[cardIndex['played']].splice(cardIndex['Card Target Value'], 1)[0];
                                        Payload["Target User"] = cardIndex["User Target"];
                                        Payload["Pile"] = (cardIndex['State'] == "Hand" ? "Hand" : cardIndex['Card Target Value']);
                                        ret[1]['discard'].push(discardCard);
                                        await ret[1].save();
                                        await ret[0].save();
                                });
                        };
                resolve("Done");
       	 	})
        })
	.then(async () => {
		return await client.findOne({"loginUser.username": getUsername}).exec();
	})
        .then(async (getUser) =>{
                await actionRegulator(getSocket, getUserSocket, getUser);
        });*/
}

async function actionRegulator(getSocket, getUserSocket, getUser){
	let tmpPayload = {};

	let fetchTable = table.findOne({"tableID": getUser['tableID']}).exec();
	let fetchHandCount =  userCard.countDocuments({"clientRef.loginUser.username": getUser.loginUser['username'], "state": "In Hand"}).exec()
	let fetchDeckCount = userCard.countDocuments({"clientRef.loginUser.username": {$ne: null}}).exec()
	Promise.all([fetchTable, fetchHandCount, fetchDeckCount])
	.then(async (ret) => {
		if (getUser["Turn"] > 1) {
	        	getUser["Turn"] = getUser["Turn"] - 1;
	        	getSocket.in(getUser["tableID"]).emit("Action", {"Action": "Continue Turn", "Username": getUser.loginUser['username'], "Payload": tmpPayload});
	         	return await getUser.save();
		} else {
			if (ret[1] < 5){
				await drawCard(getSocket, getUserSocket, ret[0], getUser);
				return await actionRegulator(getSocket, getUserSocket, getUser);
			} else {
				let nextActive = await nextTurn(getSocket, getUserSocket, ret[0])
				tmpPayload["Deck size"] = ret[2]
	                        getSocket.in(getUser["tableID"]).emit("Action", {"Action": "Continue Turn", "Username": nextActive.loginUser['username'], "Payload": tmpPayload});
				return nextActive;
			}
		}
	})
};

exports.gameOver = async function(getSocket, getUserSocket, getTableID){
        getSocket.in(getTableID).emit('Action', {"Action": "Game Over"});
}

exports.leaveTable = async function(getSocket, getUserSocket, getUsername, getTableID){
	await userCard.find({"clientRef.loginUser.username": getUsername}).exec()
	.then(async (fetchUserCards) => {
		for(let fetchCard of fetchUserCards){
			let payload = {};
			fetchCard["clientRef"] = null;
			fetchCard["state"] = "Discard";
                        payload["Target User"] = getUsername
			payload["Card ID"] = fetchCard["_id"];
                        getSocket.in(getTargetID).emit('Action', {"Action": "Discard", "Username": getUsername, "Payload": payload});
			await fetchCard.save();
		}
	})
	.then(async () => {
                return await user.findOne({"username": getUsername}).exec();
        })
        .then(async (fetchUser) => {
                let resetStatus = await resetClient(getUserSocket, fetchUser);
                getUserSocket.join("Main Lobby");
                getUserSocket.leave(getTableID);
                return resetStatus
        })
	.then(async (fetchReset) => {
		return await emptyTableCheck(getSocket, getUserSocket, getUsername, getTableID)
	});
        
}

exports.resolveTable = async function(getSocket, getUserSocket, getUsername, getTableID){
        let fetchClient = await client.findOne({"loginUser.username": getUsername}).exec()
        .then(async (fetchClient) => {
                fetchClient["PurpleCount"] = params["Purple Count"];
                fetchClient["Score"] = params["Score"];
                fetchClient["State"] = "Resolved";
                return await fetchClient.save();
        });

        await client.countDocuments({"tableID": getTableID, "State": {$ne: "Resolved"}}).exec()
        .then(async (fetchCount) => {
                if (fetchCount == 0){
                        return await client.find({"tableID": getTableID})
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
        });
}

async function nextTurn(getSocket, getUserSocket, getTable){
        return await client.findOne({"loginUser.username": getTable['activeUser']}).exec()
        .then(async (fetchClient) => {
                fetchClient["State"] = "Inactive";
		fetchClient["Turn"] = 1;
                return await fetchClient.save();
        })
        .then(async (fetchUser) => {
                let index = getTable.order.indexOf(fetchUser.loginUser['username']);
                getTable['activeUser'] = getTable.order[(index+1)%getTable.order.length];
                await getTable.save();
                return await client.findOne({"loginUser.username": getTable['activeUser']}).exec();
        })
        .then(async (fetchNextClient) => {
                if (fetchNextClient["State"] != "Preparing") fetchNextClient["State"] = "Active";
                let payload = {};
                payload["State"] = fetchNextClient["State"];
//                socket.in(getTableID).emit('Action', {"Action": "Next Turn", "Username": fetchNextClient.loginUser['username'], "Payload": payload});
                return await fetchNextClient.save();
        });
}


async function emptyTableCheck(getSocket, getUserSocket, getUsername, getTableID) {
        return await client.countDocuments({"tableID": getTableID}).exec()
        .then(async (fetchCount) => {
                console.log(fetchCount + " " + getTableID);
                if (fetchCount < 2 && getTableID != "Main Lobby"){
                        getSocket.in("Main Lobby").emit('Lobby', {"Action": "Room Closed", "Room": getTableID});
                        getSocket.in(getTableID).emit('Action', {"Action": "Game Over", "Room": getTableID});
                        return await table.deleteOne({"tableID": getTableID});
                }
		return userCard.deleteMany({"tableRef.tableID": getTableID});
        })
};

async function resetClient(getUserSocket, getUser) {
        return await client.findOne({"userID": getUserSocket.id}).exec()
        .then(async (fetchClient) => {
                fetchClient["loginUser"] = getUser;
                fetchClient["State"] = "Main Lobby";
                fetchClient["lastPlayed"] = null;
                fetchClient["played"] = [];
                fetchClient["Turn"] = 1;
                fetchClient["tableID"] = "Main Lobby";
                getUserSocket.join("Main Lobby");
                return await fetchClient.save();

        });
};

async function drawCard(getSocket, getUserSocket, getTable, getClient){
	let payload = {};
	let fetchCard = await userCard.findOne({'tableRef.tableID': getTable['tableID'], 'clientRef': null}).exec()
	.then(async (cardFetch) => {
		cardFetch['clientRef'] = getClient;
		cardFetch['state'] = "In Hand";
		return await cardFetch.save();
	});

	payload["Card Value"] = -1;
	payload["_id"] = fetchCard['_id'];
	getUserSocket.broadcast.to(getClient['tableID']).emit('Action', {"Action": "Draw", "Username": getClient.loginUser['username'], "Payload": payload});
	payload["category"] = fetchCard.cardRef['category'];
	payload["name"] = fetchCard.cardRef['name'];
//	console.log("Draw Card");
//	console.log(payload);
//	console.log(getClient);
	getUserSocket.emit('Action', {"Action": "Draw", "Username": getClient.loginUser['username'], "Payload": payload});
};

async function calculateScore(getClient, getCard)
{
	console.log("Before User: " + getClient.loginUser['username'] + " , Card: " + getCard.cardRef['name'] + " , Score: " + getClient['score']);
        switch(getCard.cardRef['category']){
                case "Blue":
                         await userCard.countDocuments({"clientRef.loginUser.username": getClient.loginUser['username'], "tableRef.tableID": getClient['tableID'], "state": "Played", "cardRef.category": "Blue"}).exec()
                         .then(async (fetchCards) => {
                                if (fetchCards > 0 && fetchCards%2 == 0) getClient['score'] += 5;
                         });
			break;
                case "Yellow":
                        await userCard.countDocuments({"clientRef.loginUser.username": getClient.loginUser['username'], "tableRef.tableID": getClient['tableID'], "state": "Played", "cardRef.category": "Yellow"}).exec()
                         .then(async (fetchCards) => {
                                if (fetchCards > 0 && fetchCards%3 == 0) getClient['score'] += 10;
                         });
			break;
                case "Purple":
                        await userCard.aggregate()
                        .match({"tableRef.tableID": getClient['tableID'], "state": "Played", "cardRef.category": "Purple" })
                        .group({_id: "$clientRef.loginUser.username", count: { $sum: 1 }}).exec()
                        .then(async (purpleCombat) => {
				console.log(purpleCombat);
                                if (purpleCombat.length == 1) {
					if (purpleCombat[0].count == 1) {
						getClient['score'] += 15;
					}
				} else {
					purpleCombat.sort((pc1, pc2) => pc2.count - pc1.count);
	       	  	                if (purpleCombat[0].count-1 == purpleCombat[1].count ) 
					{
						if (purpleCombat[0]._id == getClient.loginUser['username']) {
							getClient['score'] += 15;
							
						}
					} else if (purpleCombat[0].count == purpleCombat[1].count)
					{
						if (purpleCombat.Count > 2){
							if (purpleCombat[1].count != purpleCombat[2].count){
								await client.findOne({'loginUser.username': purpleCombat[1]._id}).exec()
								.then(async (fetchClient) => {
									fetchClient['score'] -= 15;
                        						let playload = {};
									payload['score'] = fetchClient['score'];
									getSocket.in(getTableID).emit('Action', {"Action": "Update Score", "Username": purpleCombat[1]._id, "Payload": payload});
									return await fetchClient.save();
								});
							}
						} else if (purpleCombat.Count == 2){
							await client.findOne({'loginUser.username': purpleCombat[1]._id}).exec()
							.then(async (fetchClient) => {
								fetchClient['score'] -= 15;
									payload['score'] = fetchClient['score'];
									getSocket.in(getTableID).emit('Action', {"Action": "Update Score", "Username": purpleCombat[1]._id, "Payload": payload});
								return await fetchClient.save();
							});						
						}
					}
				}
                        })
			break;                
                case "Brown":
                        await userCard.countDocuments({"clientRef.loginUser.username": getClient.loginUser['username'], "tableRef.tableID": getClient['tableID'], "state": "Played", "cardRef.category": "Brown"}).exec()
                         .then(async (fetchCards) => {
                                if (fetchCards == 1) getClient['score'] += 12;
                                if (fetchCards == 2) getClient['score'] -= 12;
                         });
			break;
                default:             
                        switch(getCard.cardRef['name']){                        
                                case "Hot Chocolate":
					getClient['score'] -= 2;
                                        break;
                                case "Fruit Tea":
					getClient['score'] -= 3;
                                        break;
                                case "Roobios":
					getClient['score'] -= 4;
                                        break;
                                case "Earl Grey":
					getClient['score'] += 2;
                                        break;
                                case "Green Tea":
					getClient['score'] += 3;
                                        break;
                                case "English Breakfast":
					getClient['score'] += 4;
                                        break;
                        }
		break;
        }
	await getClient.save();
	console.log("After User: " + getClient.loginUser['username'] + " , Card: " + getCard.cardRef['name'] + " , Score: " + getClient['score']);
	return getClient['score'];
}
