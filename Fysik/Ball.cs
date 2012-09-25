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
		public float startVelocityMultiplyer;
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
			float m12 = (float)Math.Sin(_radian);
			float m21 = -(float)Math.Sin(_radian);
			float m22 = (float)Math.Cos(_radian);

			return new Matrix2x2(m11, m12, m21, m22);
		}

		public static Matrix2x2 AntiRotation(double _radian)
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

		public int ID;
		public float radian;
		public Vector2 oldVelocity, newVelocity, startVel, centerPos, startPos;
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

			oldVelocity = new Vector2(0.0f, 0.0f);
			newVelocity = new Vector2(0.0f, 0.0f);
			centerPos = new Vector2(posScale.Center.X, posScale.Center.Y);
			startPos = centerPos;
			startVel = new Vector2((float)rand.NextDouble() * _bi.startVelocityMultiplyer, (float)rand.NextDouble() * _bi.startVelocityMultiplyer);
		}

		public void Collide(Ball _ball) //_ball = colliding ball!!
		{
			double dist = (centerPos - _ball.centerPos).Length();

			if (dist <= radian * 2)
			{
				//Calculate Center positions for the balls
				centerPos = new Vector2(posScale.Center.X, posScale.Center.Y);
				_ball.centerPos = new Vector2(_ball.posScale.Center.X, _ball.posScale.Center.Y);

				//Line of action
				Vector2 loa = (_ball.centerPos - centerPos);

				// Angle between X-axis and Line of action
				double angle = AngleBetweenInRadian(new Vector2(1.0f, 0.0f), loa); 

				//Calculate rotation matrix
				Matrix2x2 rotMat = Matrix2x2.Rotation(angle);

				//Rotate the current velocity with the rotation matrix
				Vector2 Vnp = rotMat * oldVelocity;

				//Simplifies the calculation
				double tmp = 1.0f / (_ball.bi.mass + bi.mass);

				//Calculate the new velocity vector that is parallel to the line of action
				double NVx = (bi.mass - bi.elasticity * _ball.bi.mass) * Vnp.X * tmp + (1.0f + bi.elasticity) * _ball.bi.mass * Vnp.Y * tmp;

				//Rotate Vnp to normal XY coordinates
				newVelocity += Matrix2x2.AntiRotation(angle) * Vnp;
			}
		}

		public void Update(GameTime _gameTime)
		{
			double elapsedTime = _gameTime.TotalGameTime.TotalSeconds;

			
			if (posScale.Y <= (bi.screenHeight - radian * 2))
			{
				//Gravity calculation
				posScale.Y = -(int)(startPos.Y + startVel.Y * elapsedTime - 0.5f * bi.gravity * Math.Pow(elapsedTime, 2));

				//velocity vector from collision
				posScale.X += (int)(newVelocity.X * elapsedTime);
				posScale.Y += (int)(newVelocity.Y * elapsedTime);

				oldVelocity = newVelocity;
			}
		}

		private double AngleBetweenInRadian(Vector2 _v1, Vector2 _v2)
		{
			float dot = Vector2.Dot(_v1, _v2);
			float len = _v1.Length() * _v2.Length();
			float div = dot / len;

			return Math.PI * (Math.Acos(div) * (180.0 / Math.PI)) / 180.0;
		}

		public void Draw(SpriteBatch _draw)
		{
			_draw.Draw(tex, posScale, color);
		}
	}
}
