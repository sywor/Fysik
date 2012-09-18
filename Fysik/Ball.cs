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
		public float wheight;
		public float elasticity;
		public int screenHeight;
		public int screenWidth;
	}

	public struct BallPosRect
	{
		public float X;
		public float Y;
		public float Width;
		public float Height;

		public BallPosRect(float _X, float _Y, float _Width, float _Height)
		{
			X = _X;
			Y = _Y;
			Width = _Width;
			Height = _Height;
		}

		public float Bottom
		{
			get { return (Y + Height); }
		}
		public float Top
		{
			get { return Y; }
		}
		public float Left
		{
			get { return X; }
		}
		public float Right
		{
			get { return (X + Width); }
		}
	}

	class Ball
	{
		Texture2D tex;
		Rectangle posScale;
		Random rand;
		BallInfo bi;
		float radian;
		Vector2 initVelocity, initPos;
		double elapsedTime = 0;
		

		public Ball(Texture2D _tex, BallInfo _bi)
		{
			tex = _tex;
			bi = _bi;
			
			rand = new Random();
			radian = (tex.Height * bi.scale) / 2;

			int x = rand.Next(0, bi.screenWidth - (int)radian);
			int y = rand.Next(0, bi.screenHeight - (int)radian);

			posScale = new Rectangle(x, -y, (int)(tex.Width * bi.scale), (int)(tex.Height * bi.scale));

			initPos = new Vector2(posScale.X, posScale.Y);
			initVelocity = new Vector2(0.0f, 0.0f);
		}

		public void Update(GameTime gameTime)
		{
			if (posScale.Bottom <= bi.screenHeight) //Ball above floor
			{
				elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

				posScale.Y = -((int)(initPos.Y + initVelocity.Y * elapsedTime - 0.5 * bi.gravity * Math.Pow(elapsedTime, 2)));
			}
		}

		public void Draw(SpriteBatch _draw)
		{
			_draw.Draw(tex, posScale, Color.Black);
		}
	}
}
