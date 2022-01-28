'use strict';
var mongoose = require('mongoose'),
	Schema = mongoose.Schema,
	passportLocalMongoose = require('passport-local-mongoose');


var siteUpdates = new Schema({
	message: {type:String},
	category: {type:String},
	posted: {type:Date}
});

var key = new Schema({
	key: {
		type: String
	},
	iv: {
		type: String
	}
});

var room = new Schema({
	name: {type: String}
});

var table = new Schema({
	tableID: {type: String},
	state: {type: String},
	deck: [String],
	discard: [String],
	order: [String],
	activeUser: {type: String},
	gameHost: {type: String}
});


var appliance = new Schema({
	name: {type: String},
	durability: {type: Number},
	cleanliness: {type: Number},
	start: {type: Date},
	inRoom: [room]
});



var container = new Schema({
	name: {type: String},
	capacity: {type: Number},
	durability: {type: Number}
});

var user = new Schema({
	username: {type: String},
	state: {type: String},	
	table: {type: Number},
	hand: [String],
	played: [String],
	lastPlayed: {type: String},
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
		type: String, 
		select: false
	},
});

var card = new Schema){
	cardID: {type: String},
	name: {type: String},
	value: {type: Number}
})

var userCard = new Schema({
	cardRef: {type: card},
	userRef: {type: user},
	state: {type: String},
	addon: [card]
});

var tableCard = new Schema({
	cardRef: {type: card},
	tableRef: {type: table},
	state: {type: String}
});

var client = new Schema({
	userID: {type: String},
	loginUser: {type: user},
	tableID: {type: String},
	State: {type: String},
	Turn: {type: Number},
	Score: {type: Number},
	PurpleCount: {type: Number}
});

var deck = new Schema({
	cards: [String]
});

var chatLog = new Schema({
	message: {type: String},
	type: {type: String},
	listener: [user]
});

var item = new Schema({
	name: {type:String},
	bodySlot: {type:String}
});

var roomItem = new Schema({
	inItem: [item],
	inRoom: [room]
});

var containerItem = new Schema({
	inContainer: [container],
	inItem: [item]
});

var roomContainer = new Schema({
	inContainer: [container],
	inRoom: [room]
});

var recipe = new Schema({
	name: {type: String},
	level: {type: Number},
});

var ingrediant = new Schema({
	meal: [recipe],
	mealIngrediant: [item]
});

var applianceIngrediant = new Schema({
	activeAppliance: [appliance],
	activeIngrediant: [ingrediant]
});


var humanoid = new Schema({
	name: {type: String},
	inPlayer: [user],
	inRoom: [room],
	eqHead: [item],
	eqFace: [item],
	eqLEar: [item],
	eqREar: [item],
	eqNeck: [item],
	eqLShoulder: [item],
	eqRShoulder: [item],
	eqLArm: [item],
	eqRArm: [item],
	eqLHand: [item],
	eqRHand: [item],
	eqLFinger: [item],
	eqRFinger: [item],
	eqChest: [item],
	eqBelt: [item],
	eqPant: [item],
	eqBack: [item],
	eqLFeet: [item],
	eqRFeet: [item],
	eqLKnee: [item],
	eqRKnee: [item]
});

var seat = new Schema({
	seatTable: [table],
	seatCustomer: [humanoid]
});

var login = new Schema({
	id: {type:String},
	exp:{type:Number}	
});

var roomConnection = new Schema({
	toRoom: [room],
	fromRoom: [room],
	door: {type:Boolean},
	open: {type:Boolean},
	key: {type:String}
});

user.plugin(passportLocalMongoose);
module.exports = mongoose.model("Client", client);

module.exports = mongoose.model("Deck", deck);
module.exports = mongoose.model("RMConnector", roomConnection);
module.exports = mongoose.model("Updates", siteUpdates);
module.exports = mongoose.model("Login", login);
module.exports = mongoose.model("Container", container);
module.exports = mongoose.model("ContainerItem", containerItem);
module.exports = mongoose.model("RoomContainer", roomContainer);
module.exports = mongoose.model("RoomItem", roomItem);
module.exports = mongoose.model("ActiveAppliance", applianceIngrediant);
module.exports = mongoose.model("Appliance", appliance);
module.exports = mongoose.model("Ingrediant", ingrediant);
module.exports = mongoose.model("Recipe", recipe);
module.exports = mongoose.model("Item", item);
module.exports = mongoose.model("Key", key);
module.exports = mongoose.model("Chat", chatLog); 
module.exports = mongoose.model("User", user);
module.exports = mongoose.model("Room", room);
module.exports = mongoose.model("Table", table);
module.exports = mongoose.model("Humanoid", humanoid);
module.exports = mongoose.model("Seat", seat);
module.exports = mongoose.model("Card", card);
module.exports = mongoose.model("UserCard", userCard);
module.exports = mongoose.model("TableCard", tableCard);
