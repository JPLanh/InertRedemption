'use strict'
module.exports = function(app){
	var controller = require('../Controller/gameController');
//		.post(passport.authenticate('local', { failureRedirect: '/error' }),
  //			function(req, res) {
  //			  res.redirect('/success?username='+req.user.username);
//		 	 }
//		);
	app.route("/game")
		.get(controller.getPlayers)
		.post(controller.joinGame);

	app.route("/startGame")
		.post(controller.newGame);
//	app.route("/game/:username")
//		.post(controller.joinGame);

	app.route("/gameUpdates")
		.get(controller.getGameUpdates);

	app.route("/lifeUpdates")
		.get(controller.getLifeUpdates);		

	app.route('/me')
		.get(controller.getToken);

	app.route('/ping')
		.get(controller.ping);

	app.route('/chat/:username')
		.get(controller.getChat)
		.post(controller.postChat);

	app.route('/user')
		.post(controller.newUser)
		.get(controller.existingUser);

	app.route('/user/:username')
		.get(controller.getUserRoom);

	app.route('/room')
		.post(controller.createRoom);
	
	app.route('/npc/:username')
		.get(controller.getNPC);

	app.route('/players/:username')
		.get(controller.getPlayers);

	app.route('/room/:username')
		.post(controller.enterRoom)
		.get(controller.getRoomTable);

	app.route('/roomExits/:userName')
		.get(controller.getRoomExits);

	app.route('/command')
		.post(controller.sendCommands);

	app.route('/tokens')
		.get(controller.getToken);

	app.route('/update')
		.post(controller.postUpdate);

	app.route('/test/:username')
		.get(controller.getRoomInfo);
};
