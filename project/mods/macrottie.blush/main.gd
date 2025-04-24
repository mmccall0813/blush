extends Node

const ID = "macrottie.blush"
const ModVersion = "1.0.0"

var blush = false
var temp_blush = false
var blush_timer: Timer

func _ready():
	print("blush mod ready")
	
	blush_timer = Timer.new()
	blush_timer.one_shot = true
	blush_timer.wait_time = 3
	add_child(blush_timer)
	blush_timer.connect("timeout", self, "_blush_timer_timeout")

func _update_blush():
	var player = _get_local_player()
	if player == null: return
	
	player.spoof_blush = blush || temp_blush
	player._update_animation_data()
	
	player._share_animation_data()

func _unhandled_input(event):
	if event is InputEventKey:
		if event.pressed and event.scancode == KEY_U and not event.echo:
			var player = _get_local_player()
			if player != null:
				blush = !blush
				
				_update_blush()
				
				if blush: 
					PlayerData._send_notification("forced blush is on") 
				else: 
					PlayerData._send_notification("forced blush is off")

func _blush_timer_timeout():
	temp_blush = false
	
	if !blush: _update_blush()

func _physics_process(delta):
	if Engine.get_physics_frames() % 4 == 0:
		var players = get_tree().get_nodes_in_group("player")
		var local_player = _get_local_player()
		if local_player == null: return
		
		for player in players:
			if player.is_in_group("controlled_player"): continue
			if !player.face.find_node("mouth_r").texture.load_path.begins_with("res://.import/mouth_base29.png"): continue
			
			if local_player.global_transform.origin.distance_squared_to(player.global_transform.origin) <= 3:
				temp_blush = true
				_update_blush()
				blush_timer.start()

func _get_local_player():
	var local_player = get_tree().get_nodes_in_group("controlled_player")
	if local_player.size() == 1: return local_player[0]
	return null
