'use strict';
var mongoose = require('mongoose'),
	Schema = mongoose.Schema;

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
	lobbyID: {type: String},
	started: {type: Boolean}
});

var upgrades = new Schema({
	name: {type: String},
	type: {type: String},
	user: {type: String},
	level: {type: Number},
	team: {type: String}
})

module.exports = mongoose.model("Item", item);
module.exports = mongoose.model("Lobby", lobby);
module.exports = mongoose.model("Player", player);
module.exports = mongoose.model("User", user);
module.exports = mongoose.model("Resources", resource);
module.exports = mongoose.model("Upgrade", upgrades);
