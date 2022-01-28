'use strict';
var mongoose = require('mongoose'),
	Schema = mongoose.Schema,
	passportLocalMongoose = require('passport-local-mongoose');

var table = new Schema({
	tableID: {type: String},
	state: {type: String},
	order: [String],
	activeUser: {type: String},
	gameHost: {type: String}
});

var user = new Schema({
	username: {type: String},
	image: {type:String},
	key: 
	{
		type: String,
		select: false
	},
	iv: 
	{
		type: String,
		select: false
	},
	password: 
	{
		type: String
	},
});

//Add in gears and gear attributes model and building

var client = new Schema({
	userID: {type: String},
	loginUser: {type: user},
	tableID: {type: String},
	State: {type: String},
	Turn: {type: Number},
	score: {type: Number},
	PurpleCount: {type: Number}
});

var card = new Schema({
	name: {type: String},
	category: {type: String}
})

var userCard = new Schema({
	cardRef: {type: card},
	clientRef: {type: client},
	tableRef: {type: table},
	state: {type: String},
	addon: [card]
});

var player = new Schema({
	name: {type: String},
	UserID: {type: String},
	State: {type: String},
	WeaponState: {type: String},
	lobbyID: {type: String},
	host: {type: Boolean},
	Team: {type: String},
	health: {type: Number},
	xPos: {type: Number},
	yPos: {type: Number},
	zPos: {type: Number},
	xRot: {type: Number},
	yRot: {type: Number},
	zRot: {type: Number},
	wRot: {type: Number}
});

var resource = new Schema({
	UID: {type: String},
        xPos: {type: Number},
        yPos: {type: Number},
        zPos: {type: Number},
        xRot: {type: Number},
        yRot: {type: Number},
        zRot: {type: Number},
	lobbyID: {type: String},
	amount: {type: Number},
	resource: {type: String},
	durability: {type: Number}
});

var item = new Schema({
	UID: {type: String},
	name: {type: String},	
	resource: {type: String},
        xPos: {type: Number},
        yPos: {type: Number},
        zPos: {type: Number},
        xRot: {type: Number},
        yRot: {type: Number},
        zRot: {type: Number},
        lobbyID: {type: String}
});


var lobby = new Schema({
	hostID: {type: String},
	lobbyID: {type: String},
	resourceLimit: {type: Number}
});

user.plugin(passportLocalMongoose);

module.exports = mongoose.model("Item", item);
module.exports = mongoose.model("Lobby", lobby);
module.exports = mongoose.model("Player", player);
module.exports = mongoose.model("Client", client);
module.exports = mongoose.model("User", user);
module.exports = mongoose.model("Table", table);
module.exports = mongoose.model("Card", card);
module.exports = mongoose.model("UserCard", userCard);
module.exports = mongoose.model("Resources", resource);
