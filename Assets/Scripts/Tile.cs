using UnityEngine;
using System.Collections;

/**
 * 
 * 
 *  Our terrain.png is 256 X 256 pixel
 *  each tile is 16 X 16 pixel
 * 
 * 左上角是 (0, 0) 原点
 * (0, 0)  (1, 0) (2, 0)
 * (0, 1)  (1, 1) ...
 * 
 */
public class BlockTile {
	public Vector2 left_00;
	public Vector2 left_01;
	public Vector2 left_10;
	public Vector2 left_11;

	public Vector2 right_00;
	public Vector2 right_01;
	public Vector2 right_10;
	public Vector2 right_11;

	public Vector2 top_00;
	public Vector2 top_01;
	public Vector2 top_10;
	public Vector2 top_11;

	public Vector2 bottom_00;
	public Vector2 bottom_01;
	public Vector2 bottom_10;
	public Vector2 bottom_11;

	public Vector2 front_00;
	public Vector2 front_01;
	public Vector2 front_10;
	public Vector2 front_11;

	public Vector2 back_00;
	public Vector2 back_01;
	public Vector2 back_10;
	public Vector2 back_11;

	public BlockTile(int left_x, int left_y,
		int right_x, int right_y,
		int top_x, int top_y, 
		int bottom_x, int bottom_y,
		int front_x, int front_y, 
		int back_x, int back_y){
		float texture_width = 384f;
		float texture_height = 512f;
		float tile_size = 16f;

		left_00 = new Vector2(left_x * tile_size / texture_width, 1-(left_y + 1) * tile_size / texture_height);
		left_01 = new Vector2(left_x * tile_size / texture_width, 1-left_y * tile_size / texture_height);
		left_10 = new Vector2((left_x + 1) * tile_size / texture_width, 1-(left_y + 1) * tile_size / texture_height);
		left_11 = new Vector2((left_x + 1) * tile_size / texture_width, 1-left_y * tile_size / texture_height);

		right_00 = new Vector2(right_x * tile_size / texture_width, 1-(right_y + 1) * tile_size / texture_height);
		right_01 = new Vector2(right_x * tile_size / texture_width, 1-right_y * tile_size / texture_height);
		right_10 = new Vector2((right_x + 1) * tile_size / texture_width, 1-(right_y + 1) * tile_size / texture_height);
		right_11 = new Vector2((right_x + 1) * tile_size / texture_width, 1-right_y * tile_size / texture_height);

		top_00 = new Vector2(top_x * tile_size / texture_width, 1-((top_y+1) * tile_size) / texture_height);
		top_01 = new Vector2(top_x * tile_size / texture_width, 1-(top_y * tile_size) / texture_height);
		top_10 = new Vector2((top_x + 1) * tile_size / texture_width, 1-((top_y+1) * tile_size) / texture_height);
		top_11 = new Vector2((top_x + 1) * tile_size / texture_width, 1-(top_y * tile_size) / texture_height);

		bottom_00 = new Vector2(bottom_x * tile_size / texture_width, (bottom_y + 1) * tile_size / texture_height);
		bottom_01 = new Vector2(bottom_x * tile_size / texture_width, bottom_y * tile_size / texture_height);
		bottom_10 = new Vector2((bottom_x + 1) * tile_size / texture_width, (bottom_y + 1) * tile_size / texture_height);
		bottom_11 = new Vector2((bottom_x + 1) * tile_size / texture_width, bottom_y * tile_size / texture_height);

		front_00 = new Vector2(front_x * tile_size / texture_width, 1-(front_y + 1) * tile_size / texture_height);
		front_01 = new Vector2(front_x * tile_size / texture_width, 1-front_y * tile_size / texture_height);
		front_10 = new Vector2((front_x + 1) * tile_size / texture_width, 1-(front_y + 1) * tile_size / texture_height);
		front_11 = new Vector2((front_x + 1) * tile_size / texture_width, 1-front_y * tile_size / texture_height);
			
		back_00 = new Vector2(back_x * tile_size / texture_width, 1-(back_y + 1) * tile_size / texture_height);
		back_01 = new Vector2(back_x * tile_size / texture_width, 1-back_y * tile_size / texture_height);
		back_10 = new Vector2((back_x + 1) * tile_size / texture_width, 1-(back_y + 1) * tile_size / texture_height);
		back_11 = new Vector2((back_x + 1) * tile_size / texture_width, 1-back_y * tile_size / texture_height);
	}




}
