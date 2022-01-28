'use strict';

const mongoose = require('mongoose'),
        user = mongoose.model("User"),
        client = mongoose.model("Client"),
        table = mongoose.model("Table"),
	card = mongoose.model("Card"),
	userCard = mongoose.model("UserCard"),
        crypto = require('crypto'),
        fs = require('fs'),
        jwt = require('jsonwebtoken'),
        algorithm = 'aes-256-cbc';

exports.disconnecting = async function(getSocket, getUserSocket){
	await client.findOne({"userID": getUserSocket.id}).exec()
	.then(async (fetchClient) => {
		if (fetchClient.loginUser != null)
			 getSocket.to(fetchClient['tableID']).emit('Action', {"Action": "Exit", "Username": fetchClient.loginUser['username']});
	});
}

exports.disconnect = async function(getSocket, getUserSocket){
        await client.findOne({"userID": getUserSocket.id}).exec()
	.then(async (fetchClient) => {
		//console.log(fetchClient);
	        const delRes = await client.deleteOne({"userID": getUserSocket.id});
	        if (fetchClient.loginUser != null) await emptyTableCheck(fetchClient.loginUser["username"], fetchClient.tableID, getSocket); 
	})
        console.log('user disconnected');
}

exports.registerCheckClient = async function(getSocket, getUserSocket, getUsername, getPassword){
        return await user.countDocuments({"username": getUsername}).exec()
        .then(async (fetchUser) => {
                if (fetchUser > 0)
                        getUserSocket.emit('Register', {"Action": "User Exists", "Username": getUsername, "Message": "User currently exists"});
                else
                        getUserSocket.emit('Register', {"Action": "Register Proceed", "Username": getUsername, "Message": getPassword});
        });
};

exports.registerValidateClient = async function(getSocket, getUserSocket, getUsername, getPassword){
        await client.findOne({"userID": getSocket.id}).exec()
        .then(async (fetchClient) => {
                var new_key = {key: (crypto.randomBytes(32)).toString('hex'), iv: (crypto.randomBytes(16)).toString('hex')}
                var cryptoKey = Buffer.from(new_key.key, 'hex');
                var cryptoIv = Buffer.from(new_key.iv, 'hex');
                let cipher = crypto.createCipheriv('aes-256-cbc', cryptoKey, cryptoIv);
                let cipherText = cipher.update(getPassword + "A very hardcoded delicious salt");
                cipherText = Buffer.concat([cipherText, cipher.final()]);
                let password = cipherText.toString('hex');
                let fetchUser = await new user({"username": getUsername, "password": password, "key": new_key.key, "iv": new_key.iv}).save();
                getUserSocket.emit('Login', {"Action": "Login Proceed", "Username": getUsername, "Message": getPassword});
                let resetStatus = resetClient(getSocket, fetchUser);
        });
};

exports.loginCheckClient = async function(getSocket, getUserSocket, getUsername, getPassword){
        return await client.countDocuments({"loginUser.username": getUsername}).exec()
        .then((fetchClient) => {
                if (fetchClient > 0)
                        getUserSocket.emit('Login', {"Action": "User Active", "Username": getUsername, "Message": "User is currently logged on."});
                else
                        getUserSocket.emit('Login', {"Action": "Login Proceed", "Username": getUsername, "Message": getPassword});
        });
}

exports.loginValidateClient = async function(getSocket, getUserSocket, getUsername, getPassword){
        let fetchUser = await user.findOne({"username": getUsername}).select("+password +iv +key").exec();
        let fetchClient = await client.findOne({"userID": getSocket.id}).exec();
        Promise.all([fetchUser, fetchClient])
        .then(async (res) => {
                if (res[0] == null) {
                        getUserSocket.emit('Login', {"Action": "Bad Credential"});
                } else {
                        var cryptoKey = Buffer.from(res[0]['key'], 'hex');
                        var cryptoIv = Buffer.from(res[0]['iv'], 'hex');
                        let cipher = crypto.createCipheriv('aes-256-cbc', cryptoKey, cryptoIv);
                        let cipherText = cipher.update(getPassword + "A very hardcoded delicious salt");
                        cipherText = Buffer.concat([cipherText, cipher.final()]);
                        let password = cipherText.toString('hex');
                        if (password == res[0]['password']){
                                getUserSocket.emit('Login', {"Action": "Validated", "Username": getUsername});
                        } else {
                                getUserSocket.emit('Login', {"Action": "Bad Credential"});
                        }
                }
        })
}

exports.loginClient = async function(getSocket, getUserSocket, getUsername, getPassword){
        return await user.findOne({"username": getUsername}).select(['-__v', '-_id']).exec()
        .then(async (fetchUser) => {
                let resetStatus = resetClient(getUserSocket, fetchUser);
                getUserSocket.nickname = getUsername;
                getUserSocket.emit('Login', {"Action": "Welcome", "Username": getUsername});
        });
}

async function resetClient(getUserSocket, getUser) {
        return await client.findOne({"userID": getUserSocket.id}).exec()
        .then(async (fetchClient) => {
		//console.log(fetchClient);
                fetchClient["loginUser"] = getUser;
                fetchClient["State"] = "Main Lobby";
                fetchClient["lastPlayed"] = null;
                fetchClient["played"] = [];
                fetchClient["Turn"] = 1;
		fetchClient["score"] = 0;
                fetchClient["tableID"] = "Main Lobby";
                getUserSocket.join("Main Lobby");
                return await fetchClient.save();

        });
};

async function emptyTableCheck(username, getTable, socket) {
        return await client.countDocuments({"tableID": getTable}).exec()
        .then(async (getCount) => {
                if (getCount < 2 && getTable != "Main Lobby"){
                        socket.in("Main Lobby").emit('Lobby', {"Action": "Room Closed", "Room": getTable});
                        socket.in(getTable).emit('Action', {"Action": "Game Over", "Room": getTable});
                        return await table.deleteOne({"tableID": getTable});
                }
        })
	.then(async () => {
		return userCard.deleteMany({'tableRef.tableID': getTable}).exec();
	})
};

