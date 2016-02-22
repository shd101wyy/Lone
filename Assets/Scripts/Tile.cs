using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


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

	public BlockTile(SpriteData left, SpriteData right, SpriteData top, SpriteData bottom, SpriteData front, SpriteData back){
		left_00 = left._00;
		left_01 = left._01;
		left_10 = left._10;
		left_11 = left._11;

		right_00 = right._00;
		right_01 = right._01;
		right_10 = right._10;
		right_11 = right._11;

		top_00 = top._00;
		top_01 = top._01;
		top_10 = top._10;
		top_11 = top._11;

		bottom_00 = bottom._00;
		bottom_01 = bottom._01;
		bottom_10 = bottom._10;
		bottom_11 = bottom._11;

		front_00 = front._00;
		front_01 = front._01;
		front_10 = front._10;
		front_11 = front._11;
			
		back_00 = back._00;
		back_01 = back._01;
		back_10 = back._10;
		back_11 = back._11;
	}




}
