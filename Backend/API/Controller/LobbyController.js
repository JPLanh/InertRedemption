const mongoose = require('mongoose'),
        user = mongoose.model("User"),
        client = mongoose.model("Client"),
        table = mongoose.model("Table"),
	card = mongoose.model("Card"),
	userCard = mongoose.model("UserCard");

exports.createTable = async function(getSocket, getUserSocket, getUsername, getTableID){
        getUserSocket.leave("Main Lobby");
        getUserSocket.join(getTableID);
        let fetchClient = client.findOne({"loginUser.username": getUsername}).exec();
        let fetchListOfClients = new Promise(async (resolve, reject) => 
        {
                let userList = [];
                Object.keys(getSocket.sockets.adapter.rooms[getTableID].sockets).forEach(async (perClient) => {
                        await userList.push(getSocket.sockets.connected[perClient].nickname);
                });              
//		console.log(userList); 
                await resolve(userList);
        });

        Promise.all([fetchClient, fetchListOfClients])
        .then(async (ret) => {
                ret[0]["tableID"] = getTableID;
                ret[0]["State"] = "In Lobby";
                ret[0].save();
                getUserSocket.emit('Lobby', {"Action": "Create", "Username": getUsername, "Room": getTableID, "UserList": ret[1]});
                return fetchListOfClients;
        }).then(async (listOfClient) => {
                getSocket.in("Main Lobby").emit('Lobby', {"Action": "New Room", "Room": getTableID});
                return await newGame(getUsername, listOfClient, getTableID)
        });
};

exports.joinTable = async function(getSocket, getUserSocket, getUsername, getTableID){
        getUserSocket.leave("Main Lobby");
        getUserSocket.join(getTableID);
        let fetchClient = client.findOne({"loginUser.username": getUsername}).exec();
        let fetchTable = table.findOne({"tableID": getTableID}).exec();
        let fetchListOfClients = new Promise(async (resolve, reject) => 
        {
                let userList = [];
                Object.keys(getSocket.sockets.adapter.rooms[getTableID].sockets).forEach(async (perClient) => {
                        await userList.push(getSocket.sockets.connected[perClient].nickname);
                });               
                resolve(userList);
        });

        Promise.all([fetchClient, fetchListOfClients, fetchTable])
        .then(async (ret) => {
                let payload = {};
                ret[0]["tableID"] = getTableID;
                ret[0]["State"] = "In Lobby";
                ret[0].save();
                getSocket.in(getTableID).emit('Action', {"Action": "Join", "Username": getUsername,"UserList": ret[1], "Room": getTableID});
                return ret;
        })
        .then(async (ret) => {
                ret[2]['order'] = ret[1];
                return await ret[2].save();
        });
};

exports.beginTable = async function(getSocket, getUserSocket, getUsername, getTableID){
        table.findOne({"tableID": getTableID}).exec()
        .then(async (fetchTable) => {
                fetchTable["state"] = "New";
                await fetchTable.save();
        });
        getSocket.in(getTableID).emit('Action', {"Action": "Begin Game", "Username": getUsername, "Room": getTableID});
};

exports.refreshTableList = async function(getSocket, getUserSocket, getUsername, getTableID){
        var tableList = [];
        table.find({"state": "Waiting"}).exec()
        .then(async (fetchTables) => {
                return await new Promise(async (resolve, reject) => {
                        let tempList = [];
                        fetchTables.forEach((perTable) => {
                                tempList.push(perTable.tableID);
                        });
                        await resolve(tempList);
                });
        })
        .then(async (tableList) => {
                getUserSocket.emit('Lobby', {"Action": "Refresh Tables", "UserList": tableList});
        });
};

async function createCards(){
        var mapper = {
                "Iced Coffee":{"amount":5, "category": "Blue"},
                "Iced Tea":{"amount":5, "category": "Blue"},
                "Cold Brew":{"amount":4, "category": "Brown"},
                "Complimentary Cookie":{"amount":4, "category": "Action"},
                "Drinks Carrier":{"amount":2, "category": "Action"},
                "Drink Spill":{"amount":4, "category": "Action"},
                "Hot Chocolate":{"amount":5, "category": "Green"},
                "Fruit Tea":{"amount":3, "category": "Green"},
                "Roobios":{"amount":2, "category": "Green"},
                "Loyalty Card":{"amount":2, "category": "Action"},
                "Earl Grey":{"amount":5, "category": "Orange"},
                "Green Tea":{"amount":4, "category": "Orange"},
                "English Breakfast":{"amount":3, "category": "Orange"},
                "Single Espresso":{"amount":10, "category": "Purple"},
                "Double Espresso":{"amount":5, "category": "Purple"},
                "To-Go Cup":{"amount":4, "category": "Action"},
                "Cappuccino":{"amount":5, "category": "Yellow"},
                "Latte":{"amount":5, "category": "Yellow"},
                "Flat White":{"amount":5, "category": "Yellow"}

        };

	Object.keys(mapper).forEach(async (key) => {
		for (var cardAmt = 0; cardAmt < mapper[key].amount; cardAmt++){
                	await new card({"name": key, "category": mapper[key].category}).save()
		}
	});
}
async function newGame(host, order, getTableID){
        console.log("new game");
        shuffle(order);
        shuffle(order);
	
        let fetchTable =  new table({"tableID": getTableID,"order": order, "activeUser": order[0], "gameHost": host, "state": "Waiting"}).save()
	let fetchCards = card.find({}).exec();

	Promise.all([fetchTable, fetchCards])
	.then(async (ret) => {
		shuffle(ret[1]);
		shuffle(ret[1]);
		for(let cardIter of ret[1]){
			await new userCard({"cardRef": cardIter, "tableRef": ret[0], "state": "Draw Pile"}).save();
		}
	})
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

