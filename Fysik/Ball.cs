using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Fysik
{
	public struct BallInfo
	{
		public float scale;
		public float gravity;
		public float mass;
		public float elasticity;
		public int screenHeight;
		public int screenWidth;
	}

	public struct Matrix2x2
	{
		float m11, m12, m21, m22;

		public Matrix2x2(float _m11, float _m12, float _m21, float _m22)
		{
			m11 = _m11;
			m12 = _m12;
			m21 = _m21;
			m22 = _m22;
		}

		public static Matrix2x2 operator * (Matrix2x2 _v1, Matrix2x2 _v2)
		{
			float m11 = _v1.m11 * _v2.m11 + _v1.m12 * _v2.m21;
			float m12 = _v1.m11 * _v2.m12 + _v1.m12 * _v2.m22;
			float m21 = _v1.m21 * _v2.m11 + _v1.m22 * _v2.m21;
			float m22 = _v1.m21 * _v2.m12 + _v1.m22 * _v2.m22;

			return new Matrix2x2(m11, m12, m21, m22);
		}

		public static Vector2 operator * (Matrix2x2 _v1, Vector2 _v2)
		{
			float m1 = _v1.m11 * _v2.X + _v1.m12 * _v2.Y;
			float m2 = _v1.m21 * _v2.X + _v1.m22 * _v2.Y;

			return new Vector2(m1, m2);
		}

		public static Matrix2x2 Rotation(double _radian)
		{
			float m11 = (float)Math.Cos(_radian);
			float m12 = -(float)Math.Sin(_radian);
			float m21 = (float)Math.Sin(_radian);
			float m22 = (float)Math.Cos(_radian);

			return new Matrix2x2(m11, m12, m21, m22);
		}
	}

	class Ball
	{
		Texture2D tex;		
		Random rand;		
		Color color;
		List<Ball> collidningBalls = new List<Ball>();
		Vector2 myNewPos;

		public int ID;
		public float radian;
		public Vector2 centerPos, oldVelocity, newVelocity;
		public BallInfo bi;
		public Rectangle posScale;

		public Ball(Texture2D _tex, BallInfo _bi, Rectangle _spawnBox, Color _color, int _ID)
		{
			tex = _tex;
			bi = _bi;
			color = _color;
			ID = _ID;

			rand = new Random();
			radian = (tex.Height * bi.scale) / 2;

			int x = rand.Next(0, _spawnBox.Width - (int)radian);
			int y = rand.Next(0, _spawnBox.Height - (int)radian);

			posScale = new Rectangle(x, -y, (int)(tex.Width * bi.scale), (int)(tex.Height * bi.scale));
			centerPos = new Vector2(posScale.X + (int)radian, posScale.Y + (int)radian);
			myNewPos = new Vector2(posScale.X, posScale.Y);
			oldVelocity = new Vector2(0.0f, -1.0f);
			newVelocity = new Vector2(0.0f, -1.0f);
		}

		public void Collide(Ball _ball)
		{
			if ((_ball.centerPos - centerPos).Length() <= radian * 2 && _ball.ID != ID) //Collision
			{
				Vector2 loa = (_ball.centerPos - centerPos); //Line of action
				double angle = AngleBetween(new Vector2(1.0f, 0.0f), loa); // Angle between X-axis and Line of action

				double V1p = _ball.oldVelocity.X * Math.Cos(angle); //Ball 1
				double V1n = -_ball.oldVelocity.Y * Math.Sin(angle);

				//double V2p = velocity.X * Math.Sin(angle); //Ball 2
				//double V2n = velocity.Y * Math.Cos(angle);

				double V1pp = ((_ball.bi.mass - bi.elasticity * bi.mass) / (_ball.bi.mass + bi.mass)) * V1p; //Längs med loa Ball1
				//double V2pp = (((1 + bi.elasticity) * _ball.bi.mass) / (_ball.bi.mass + bi.mass)) * V2p; //Längs med loa Ball2

				double V1xp = V1pp * Math.Cos(angle) - V1n * Math.Sin(angle);
				double V1yp = V1pp * Math.Sin(angle) + V1n * Math.Cos(angle);

				newVelocity.X += (float)V1xp;
				newVelocity.Y += (float)V1yp;
			}
		}

		public void Update(GameTime gameTime)
		{
			double elapsedTime = gameTime.ElapsedGameTime.TotalSeconds;

			posScale.Y = (int)(posScale.Y + oldVelocity.Y * elapsedTime - 0.5 * bi.gravity * Math.Pow(elapsedTime, 2));
			posScale.X = (int)(posScale.X + oldVelocity.X * elapsedTime);

			oldVelocity = newVelocity;
		}

		private double AngleBetween(Vector2 _v1, Vector2 _v2)
		{
			float dot = Vector2.Dot(_v1, _v2);
			float len = _v1.Length() * _v2.Length();
			float div = dot / len;

			return Math.Acos(div) * (180.0 / Math.PI);
		}

		public void Draw(SpriteBatch _draw)
		{
			_draw.Draw(tex, posScale, color);
		}
	}
}
