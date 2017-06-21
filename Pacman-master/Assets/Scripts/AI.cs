using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Behavior Ghost: http://gameinternals.com/post/2072558330/understanding-pac-man-ghost-behavior
public class AI : MonoBehaviour {

	public Transform target; //vi tri cua con pacman theo tọa độ x, y --> để xác định tile

	private List<TileManager.Tile> tiles = new List<TileManager.Tile>(); // cái map để biết ngã tư, tường này nọ...
	private TileManager manager;
	public GhostMove ghost;//dùng để điểu khiển hướng di chuyển

	public TileManager.Tile nextTile = null;//TIle tiếp theo s đi của ghost
	public TileManager.Tile targetTile;//TILE của con pacman đang đứng
	TileManager.Tile currentTile;//Tile hiện tại của 1 con ghost

	void Awake()
	{
		manager = GameObject.Find("Game Manager").GetComponent<TileManager>();
		tiles = manager.tiles;

		//if(ghost == null)	Debug.Log ("game object ghost not found");
		//if(manager == null)	Debug.Log ("game object Game Manager not found");
	}

	public void AILogic()
	{

		// get current tile of ghost
		Vector3 currentPos = new Vector3(transform.position.x + 0.499f, transform.position.y + 0.499f);
		currentTile = tiles[manager.Index ((int)currentPos.x, (int)currentPos.y)];
		
		targetTile = GetTargetTilePerGhost();
		
		// get the next tile according to direction
		if(ghost.direction.x > 0)	nextTile = tiles[manager.Index ((int)(currentPos.x+1), (int)currentPos.y)];
		if(ghost.direction.x < 0)	nextTile = tiles[manager.Index ((int)(currentPos.x-1), (int)currentPos.y)];
		if(ghost.direction.y > 0)	nextTile = tiles[manager.Index ((int)currentPos.x, (int)(currentPos.y+1))];
		if(ghost.direction.y < 0)	nextTile = tiles[manager.Index ((int)currentPos.x, (int)(currentPos.y-1))];

		//o tiep theo phai di chuyen duoc
		if(nextTile.occupied || currentTile.isIntersection)
		{
			//---------------------
			//Chạm vào tường nga 2
			if(nextTile.occupied && !currentTile.isIntersection)
			{
				// nếu đụng tường khi đang di chuyển sang ngang
				if(ghost.direction.x != 0)
				{
					if(currentTile.down == null)	ghost.direction = Vector3.up;
					else 							ghost.direction = Vector3.down;
					
				}
				
				// nếu đụng tường khi di chuyển đứng
				else if(ghost.direction.y != 0)
				{
					if(currentTile.left == null)	ghost.direction = Vector3.right; 
					else 							ghost.direction = Vector3.left;
					
				}
				
			}
			
			//---------------------------------------------------------------------------------------//
			// Nếu đang ở giao lộ nga 3, nga 4
			if(currentTile.isIntersection)
			{
				List<TileManager.Tile> open = new List<TileManager.Tile> ();
				List<TileManager.Tile> close = new List<TileManager.Tile> ();
				//khoi tao A*
				currentTile.h = currentTile.f = manager.distance (currentTile, targetTile);
				TileManager.Tile saveCurrentTile = currentTile;
				open.Add (currentTile);


				while (open.Count != 0) {
					//tìm tile có f nhỏ nhấy trong open
					float MIN = 9999999f;
					for(int i = 0; i < open.Count; i++){
						if (open [i].f < MIN) {
							MIN = open [i].f;
							currentTile = open [i];
						}
					}
					if (targetTile.x == currentTile.x && targetTile.y == currentTile.y) {
						Debug.Log ("Break");
						break;
					}
					//thêm zô close and xóa trong open

					open.Remove (currentTile);
					close.Add (currentTile);

					//Tìm đỉnh kề cho tile hiện tại và them vào open
					TileManager.Tile p = currentTile;

					if (p.left != null) {
						//hoac giao lo, hoac goc vuong
						p = p.left;
						/*while (p.adjacentCount < 2) {
							//Debug.Log ("nga ba ben trai dau tien p:" + p.x + "" + p.y);
							if (p.left == null) {
								break;
							} else {
								p = p.left;
							}
						}*/

						//kiem tra co close, open, phai co 3 cai if
						//neu p chua co trong close va open
						if (!open.Contains (p) && !close.Contains (p)) {
							///tim duoc toi intersection hoac dung tuong
							p.before = currentTile;
							p.g = manager.distance(currentTile, p) + p.before.g;
							p.h = manager.distance(targetTile, p);
							p.f = p.g + p.h;

							open.Add (p);

						}
						//neu den dc p voi path ngan hon, cap nhat lai p trong open
						if (open.Contains (p) && p.g > currentTile.g + manager.distance(currentTile, p )) {
							p.before = currentTile;
							p.g = currentTile.g + manager.distance (currentTile, p);
							p.f = p.g + p.h;

						}
						//neu p ton tai trong close, co path ngan hon
						if (close.Contains (p) && p.g > currentTile.g + manager.distance(currentTile, p)) {
							//Debug.Log ("Update close");
							//break;
							close.Remove (p);
							p.g = manager.distance(currentTile, p) + p.before.g;
							p.h = manager.distance(targetTile, p);
							p.f = p.g + p.h;
							p.before = currentTile;
							open.Add (p);

						}

					}
					p = currentTile;
					//Debug.Log ("adjacentCount p-right:" + p.adjacentCount);
					if (p.right != null) {
						p = p.right;
						/*while (p.adjacentCount < 2) {
							//Debug.Log ("nga ba ben phai dau tien p:" + p.right.x + "" + p.right.y);
							if (p.right == null) {
								break;
							} else {
								p = p.right;
							}
						}*/

						//kiem tra co close, open, phai co 3 cai if
						//neu p chua co trong close va open
						if (!open.Contains (p) && !close.Contains (p)) {
							///tim duoc toi intersection hoac dung tuong
						    p.before = currentTile;
							p.g = manager.distance(currentTile, p) + p.before.g;
							p.h = manager.distance(targetTile, p);
							p.f = p.g + p.h;

							open.Add (p);

						}
						//neu den dc p voi path ngan hon, cap nhat lai p trong open
						if (open.Contains (p) && p.g > currentTile.g + manager.distance(currentTile, p )) {
							p.before = currentTile;
							p.g = currentTile.g + manager.distance (currentTile, p);
							p.f = p.g + p.h;

						}
						//neu p ton tai trong close, co path ngan hon
						if (close.Contains (p) && p.g > currentTile.g + manager.distance(currentTile, p)) {
							Debug.Log ("Update close");
							close.Remove (p);
							p.g = manager.distance(currentTile, p) + p.before.g;
							p.h = manager.distance(targetTile, p);
							p.f = p.g + p.h;
							p.before = currentTile;

							open.Add (p);
						}
					}
					p = currentTile;
					if (p.up != null) {
						p = p.up;
						/*while (p.adjacentCount < 2) {
							if (p.up == null) {
								break;
							} else {
								p = p.up;
							}
						}*/

						//kiem tra co close, open, phai co 3 cai if
						//neu p chua co trong close va open
						if (!open.Contains (p) && !close.Contains (p)) {
							///tim duoc toi intersection hoac dung tuong
							p.before = currentTile;
							p.g = manager.distance(currentTile, p) + p.before.g;
							p.h = manager.distance(targetTile, p);
							p.f = p.g + p.h;

							open.Add (p);

						}
						//neu den dc p voi path ngan hon, cap nhat lai p trong open
						if (open.Contains (p) && p.g > currentTile.g + manager.distance(currentTile, p )) {
							p.g = currentTile.g + manager.distance (currentTile, p);
							p.f = p.g + p.h;
							p.before = currentTile;
						}
						//neu p ton tai trong close, co path ngan hon
						if (close.Contains (p) && p.g > currentTile.g + manager.distance(currentTile, p)) {
							Debug.Log ("Update close");
							close.Remove (p);
							p.g = manager.distance(currentTile, p) + p.before.g;
							p.h = manager.distance(targetTile, p);
							p.f = p.g + p.h;
							p.before = currentTile;

							open.Add (p);
						}
					}
					p = currentTile;
					if (p.down != null) {
						p = p.down;
						/*while (p.adjacentCount < 2) {
							if (p.down == null) {
								break;
							} else {
								p = p.down;
							}
						}*/

						//kiem tra co close, open, phai co 3 cai if
						//neu p chua co trong close va open
						if (!open.Contains (p) && !close.Contains (p)) {
							///tim duoc toi intersection hoac dung tuong
							p.before = currentTile;
							p.g = manager.distance(currentTile, p) + p.before.g;
							p.h = manager.distance(targetTile, p);
							p.f = p.g + p.h;

							open.Add (p);

						}
						//neu den dc p voi path ngan hon, cap nhat lai p trong open
						if (open.Contains (p) && p.g > currentTile.g + manager.distance(currentTile, p )) {
							p.g = currentTile.g + manager.distance (currentTile, p);
							p.f = p.g + p.h;
							p.before = currentTile;
						}
						//neu p ton tai trong close, co path ngan hon
						if (close.Contains (p) && p.g > currentTile.g + manager.distance(currentTile, p)) {
							Debug.Log ("Update close");
							close.Remove (p);
							p.g = manager.distance(currentTile, p) + p.before.g;
							p.h = manager.distance(targetTile, p);
							p.f = p.g + p.h;
							p.before = currentTile;

							open.Add (p);
						}
					}

				}
				//het vong while, currentTile = targetTile, da co duong di
				//Debug.Log(close.Count);
				while (saveCurrentTile.h != currentTile.before.h) {
					currentTile = currentTile.before;
				}
				//neu nam treen truc dung
				if (saveCurrentTile.x == currentTile.x) {
					if (saveCurrentTile.y < currentTile.y) {
						ghost.direction = Vector3.up;
					} else
						ghost.direction = Vector3.down;
				} else {
					if (saveCurrentTile.x < currentTile.x) {
						ghost.direction = Vector3.right;
					} else {
						ghost.direction = Vector3.left;
					}
				}
					
				//manager.tiles
				/*dist1 = dist2 = dist3 = dist4 = 999999f;
				if(currentTile.up != null && !currentTile.up.occupied && !(ghost.direction.y < 0)) 		dist1 = manager.distance(currentTile.up, targetTile);
				if(currentTile.down != null && !currentTile.down.occupied &&  !(ghost.direction.y > 0)) 	dist2 = manager.distance(currentTile.down, targetTile);
				if(currentTile.left != null && !currentTile.left.occupied && !(ghost.direction.x > 0)) 	dist3 = manager.distance(currentTile.left, targetTile);
				if(currentTile.right != null && !currentTile.right.occupied && !(ghost.direction.x < 0))	dist4 = manager.distance(currentTile.right, targetTile);
				
				float min = Mathf.Min(dist1, dist2, dist3, dist4);
				if(min == dist1) ghost.direction = Vector3.up;
				if(min == dist2) ghost.direction = Vector3.down;
				if(min == dist3) ghost.direction = Vector3.left;
				if(min == dist4) ghost.direction = Vector3.right;*/
				
			}
			
		}
		
		// if there is no decision to be made, designate next waypoint for the ghost
		else
		{
			ghost.direction = ghost.direction;	// setter updates the waypoint
		}
	}
	//trang thai bi scatter, chay tron pacman
	public void RunLogic()
	{
		// toa do hien tai cua ghost
		Vector3 currentPos = new Vector3(transform.position.x + 0.499f, transform.position.y + 0.499f);
		currentTile = tiles[manager.Index ((int)currentPos.x, (int)currentPos.y)];

		// get tile tiep theo dua vao huong di cua ghost hien tai
		if(ghost.direction.x > 0)	nextTile = tiles[manager.Index ((int)(currentPos.x+1), (int)currentPos.y)];
		if(ghost.direction.x < 0)	nextTile = tiles[manager.Index ((int)(currentPos.x-1), (int)currentPos.y)];
		if(ghost.direction.y > 0)	nextTile = tiles[manager.Index ((int)currentPos.x, (int)(currentPos.y+1))];
		if(ghost.direction.y < 0)	nextTile = tiles[manager.Index ((int)currentPos.x, (int)(currentPos.y-1))];


		if(nextTile.occupied || currentTile.isIntersection)
		{
			//---------------------
			// neu next la tuong
			if(nextTile.occupied && !currentTile.isIntersection)
			{
				//di chuyen qua trai, qua phai va dung wall
				if(ghost.direction.x != 0)
				{
					if(currentTile.down == null)	ghost.direction = Vector3.up;
					else 							ghost.direction = Vector3.down;
					
				}
				
				// di chuyen len , xuong dung wall
				else if(ghost.direction.y != 0)
				{
					if(currentTile.left == null)	ghost.direction = Vector3.right; 
					else 							ghost.direction = Vector3.left;
					
				}
				
			}
			
			//---------------------------------------------------------------------------------------
			// neu dang o giao lo, nga 3, nga 4
			if(currentTile.isIntersection)
			{
				//
				float dist1, dist2, dist3, dist4;// khoảng cách
				/*List<TileManager.Tile> availableTiles = new List<TileManager.Tile>();
				TileManager.Tile chosenTile;
				if(currentTile.up != null && !currentTile.up.occupied && !(ghost.direction.y < 0)) 			availableTiles.Add (currentTile.up);
				if(currentTile.down != null && !currentTile.down.occupied &&  !(ghost.direction.y > 0)) 	availableTiles.Add (currentTile.down);	
				if(currentTile.left != null && !currentTile.left.occupied && !(ghost.direction.x > 0)) 		availableTiles.Add (currentTile.left);
				if(currentTile.right != null && !currentTile.right.occupied && !(ghost.direction.x < 0))	availableTiles.Add (currentTile.right);*/
				dist1 = dist2 = dist3 = dist4 = 0f;
				if (currentTile.up != null && !currentTile.up.occupied && !(ghost.direction.y < 0) && targetTile != null) {
					if (currentTile.up == null) {
						Debug.Log ("currentTile.up == null");
					}
					if (targetTile == null) {
						Debug.Log ("targetTile == null");
					}
					dist1 = manager.distance(currentTile.up, targetTile);
				}
					
				if(currentTile.down != null && !currentTile.down.occupied &&  !(ghost.direction.y > 0) && targetTile != null) 	dist2 = manager.distance(currentTile.down, targetTile);
				if(currentTile.left != null && !currentTile.left.occupied && !(ghost.direction.x > 0) && targetTile != null) 	dist3 = manager.distance(currentTile.left, targetTile);
				if(currentTile.right != null && !currentTile.right.occupied && !(ghost.direction.x < 0) && targetTile != null)	dist4 = manager.distance(currentTile.right, targetTile);
				
				float max = Mathf.Max(dist1, dist2, dist3, dist4);
				if(max == dist1) ghost.direction = Vector3.up;
				if(max == dist2) ghost.direction = Vector3.down;
				if(max == dist3) ghost.direction = Vector3.left;
				if(max == dist4) ghost.direction = Vector3.right;
				//int rand = Random.Range(0, availableTiles.Count);
				//chosenTile = availableTiles[rand];
				//ghost.direction = Vector3.Normalize(new Vector3(chosenTile.x - currentTile.x, chosenTile.y - currentTile.y, 0));
				//Debug.Log (ghost.name + ": Chosen Tile (" + chosenTile.x + ", " + chosenTile.y + ")" );
			}
			
		}
		
		// if there is no decision to be made, designate next waypoint for the ghost
		else
		{
			ghost.direction = ghost.direction;	// setter updates the waypoint
		}
	}


	TileManager.Tile GetTargetTilePerGhost()
	{
		Vector3 targetPos;
		TileManager.Tile targetTile;
		Vector3 dir;

		// get the target tile position (round it down to int so we can reach with Index() function)
		switch(name)
		{
		case "blinky":	// target = pacman
			targetPos = new Vector3 (target.position.x, target.position.y);
			targetTile = tiles[manager.Index((int)targetPos.x, (int)targetPos.y)];
			break;
		case "pinky":	// target = pacman + 4*pacman's direction (4 steps ahead of pacmann 
			//don dau truoc 4 buoc cua pacman
			dir = target.GetComponent<PlayerController>().getDir();//cho thang pacman dang dung
			targetPos = new Vector3 (target.position.x, target.position.y) + 4*dir;

			// if pacmans going up, not 4 ahead but 4 up and 4 left is the target
			// so subtract 4 from X coord from target position
			if(dir == Vector3.up)	targetPos -= new Vector3(4, 0, 0);

			targetTile = tiles[manager.Index((int)targetPos.x, (int)targetPos.y)];
			break;

		case "inky":	// target = ambushVector(pacman+2 - blinky) added to pacman+2
			dir = target.GetComponent<PlayerController> ().getDir ();
			Vector3 blinkyPos = GameObject.Find ("blinky").transform.position;
			Vector3 ambushVector = target.position + 2 * dir - blinkyPos;
			targetPos = new Vector3 (target.position.x, target.position.y) + 2 * dir + ambushVector;

			//Debug.Log ("target x = " + target.position.x + "target y = " + target.position.y);
			//Debug.Log ("blinkyPos x = " + blinkyPos.x + "blinkyPos y = " + blinkyPos.y);
			//Debug.Log ("ambushVector x = " + ambushVector.x + "ambushVector y = " + ambushVector.y);
			targetTile = tiles[manager.Index((int)targetPos.x, (int)targetPos.y)];
			break;
		case "clyde":
			targetPos = new Vector3 (target.position.x, target.position.y);
			targetTile = tiles[manager.Index((int)targetPos.x, (int)targetPos.y)];
			if(manager.distance(targetTile, currentTile) < 9)
				targetTile = tiles[manager.Index (0, 2)];
			break;
		default:
			targetTile = null;
			Debug.Log ("TARGET TILE NOT ASSIGNED");
			break;
		
		}
		return targetTile;
	}
		
}