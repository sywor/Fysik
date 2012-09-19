using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Fysik
{
	public class Game : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		SpriteFont font;
		Texture2D ballTex;
		List<Ball> balls = new List<Ball>();
		Rectangle spawnBox;
		Color[] colArr = new Color[] { Color.Black, Color.White, Color.Red, Color.Green, Color.Blue, Color.Yellow };
		Random rand = new Random();

		int totalFrames = 0;
		int fps = 0;
		int numballs = 100;
		float elapsedTime = 0.0f;

		public Game()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			graphics.PreferredBackBufferHeight = 720;
			graphics.PreferredBackBufferWidth = 1280;
			//graphics.SynchronizeWithVerticalRetrace = false;
			//IsFixedTimeStep = false;
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			font = Content.Load<SpriteFont>("font");
			ballTex = Content.Load<Texture2D>("fysikBall");

			BallInfo bi = new BallInfo();
			bi.elasticity = 1.0f;
			bi.scale = 0.3f;
			bi.gravity = 9.82f;
			bi.mass = 1.0f;
			bi.screenHeight = graphics.PreferredBackBufferHeight;
			bi.screenWidth = graphics.PreferredBackBufferWidth;

			spawnBox = new Rectangle(0, 0, bi.screenWidth, 100);

			for (int i = 0; i < numballs; i++ )
			{
				balls.Add(new Ball(ballTex, bi, spawnBox, colArr[rand.Next(0, colArr.Length)], i));
				System.Threading.Thread.Sleep(10);
			}
		}

		protected override void Update(GameTime gameTime)
		{
			elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

			if (elapsedTime >= 1000.0f)
			{
				fps = totalFrames;
				totalFrames = 0;
				elapsedTime = 0.0f;
			}

			foreach (Ball b1 in balls)
			{
				foreach (Ball b2 in balls)
				{
					b1.Collide(b2);
				}
			}

			foreach (Ball b in balls)
			{
				b.Update(gameTime);
			}


			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				this.Exit();
			}

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			totalFrames++;
			GraphicsDevice.Clear(Color.Gray);

			spriteBatch.Begin();
			spriteBatch.DrawString(font, string.Format("FPS: {0}", fps), new Vector2(10.0f, 10.0f), Color.White);

			foreach (Ball b in balls)
			{
				b.Draw(spriteBatch);
			}

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
