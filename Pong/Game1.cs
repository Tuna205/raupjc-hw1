using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Pong
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		public GenericList<Wall> Walls { get; set; }
		public GenericList<Wall> Goals { get; set; }

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferHeight = 900,
				PreferredBackBufferWidth = 500

			};
			Content.RootDirectory = "Content";
		}
		/// <summary >
		/// Bottom paddle object
		/// </summary >
		public Paddle PaddleBottom { get; private set; }
		/// <summary >
		/// Top paddle object
		/// </summary >
		public Paddle PaddleTop { get; private set; }
		/// <summary >
		/// Ball object
		/// </summary >
		public Ball Ball { get; private set; }
		/// <summary >
		/// Background image
		/// </summary >
		public Background Background { get; private set; }
		/// <summary >
		/// Sound when ball hits an obstacle .
		/// SoundEffect is a type defined in Monogame framework
		/// </summary >
		public SoundEffect HitSound { get; private set; }
		/// <summary >
		/// Background music . Song is a type defined in Monogame framework
		/// </summary >
		public Song Music { get; private set; }
		/// <summary >
		/// Generic list that holds Sprites that should be drawn on screen
		/// </summary >
		private IGenericList<Sprite> SpritesForDrawList = new GenericList<Sprite>();

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// Screen bounds details . Use this information to set up game objects positions.
			var screenBounds = GraphicsDevice.Viewport.Bounds;

			PaddleBottom = new Paddle(GameConstants.PaddleDefaultWidth, GameConstants.PaddleDefaulHeight, GameConstants.PaddleDefaulSpeed);
			PaddleBottom.X = screenBounds.Width / 2f - PaddleBottom.Width / 2f;
			PaddleBottom.Y = screenBounds.Bottom - PaddleBottom.Height;

			PaddleTop = new Paddle(GameConstants.PaddleDefaultWidth, GameConstants.PaddleDefaulHeight, GameConstants.PaddleDefaulSpeed);
			PaddleTop.X = GameConstants.ScreenWidth / 2 - GameConstants.PaddleDefaultWidth / 2;
			PaddleTop.Y = GameConstants.PaddleYOffset;

			Ball = new Ball(GameConstants.DefaultBallSize, GameConstants.DefaultInitialBallSpeed, GameConstants.DefaultBallBumpSpeedIncreaseFactor)
			{
				X = (float)GameConstants.ScreenWidth / 2,
				Y = (float)GameConstants.ScreenHeight / 2
			};
			Background = new Background(screenBounds.Width, screenBounds.Height);

			Walls = new GenericList<Wall>()
			{
				// try with 100 for default wall size !
				new Wall(-GameConstants.WallDefaultSize, 0, GameConstants.WallDefaultSize, screenBounds.Height),
				new Wall(screenBounds.Right, 0, GameConstants.WallDefaultSize, screenBounds.Height)
			};
			Goals = new GenericList<Wall>()
			{
				new Wall (0, screenBounds .Height , screenBounds .Width , GameConstants . WallDefaultSize ),
				new Wall ( screenBounds .Top ,- GameConstants . WallDefaultSize , screenBounds .Width , GameConstants . WallDefaultSize ),
			};

			// Add our game objects to the sprites that should be drawn collection .
			SpritesForDrawList.Add(Background);
			SpritesForDrawList.Add(PaddleBottom);
			SpritesForDrawList.Add(PaddleTop);
			SpritesForDrawList.Add(Ball);
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Initialize new SpriteBatch object which will be used to draw textures .
			spriteBatch = new SpriteBatch(GraphicsDevice);
			// Set textures
			Texture2D paddleTexture = Content.Load<Texture2D>("paddle");
			PaddleBottom.Texture = paddleTexture;
			PaddleTop.Texture = paddleTexture;
			Ball.Texture = Content.Load<Texture2D>("ball");
			Background.Texture = Content.Load<Texture2D>("background");
			// Load sounds
			// Start background music
			HitSound = Content.Load<SoundEffect>("hit");
			Music = Content.Load<Song>("music");
			MediaPlayer.IsRepeating = true;
			MediaPlayer.Play(Music);
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent() 
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			var touchState = Keyboard.GetState();
			if (touchState.IsKeyDown(Keys.Left))
			{
				PaddleBottom.X = PaddleBottom.X - (float)(PaddleBottom.Speed * gameTime.ElapsedGameTime.TotalMilliseconds);
			}
			if (touchState.IsKeyDown(Keys.Right))
			{
				PaddleBottom.X = PaddleBottom.X + (float)(PaddleBottom.Speed * gameTime.ElapsedGameTime.TotalMilliseconds);
			}

			if (touchState.IsKeyDown(Keys.A))
			{
				PaddleTop.X = PaddleTop.X - (float)(PaddleBottom.Speed * gameTime.ElapsedGameTime.TotalMilliseconds);
			}
			if (touchState.IsKeyDown(Keys.D))
			{
				PaddleTop.X = PaddleTop.X + (float)(PaddleBottom.Speed * gameTime.ElapsedGameTime.TotalMilliseconds);
			}
			PaddleBottom.X = MathHelper.Clamp(PaddleBottom.X, 0, 500 - PaddleBottom.Width);
			PaddleTop.X = MathHelper.Clamp(PaddleTop.X, 0, 500 - PaddleTop.Width);

			var ballPositionChange = Ball.Direction * (float)(gameTime.ElapsedGameTime.TotalMilliseconds * Ball.Speed);
			Ball.X += ballPositionChange.X;
			Ball.Y += ballPositionChange.Y;

			// Ball - side walls
			if (Walls.Any(w => CollisionDetector.Overlaps(Ball, w)))
			{
				Ball.Direction = new Vector2(-Ball.Direction.X, Ball.Direction.Y);
				Ball.Speed = Ball.Speed * Ball.BumpSpeedIncreaseFactor;
			}
			// Ball - winning walls
			if (Goals.Any(w => CollisionDetector.Overlaps(Ball, w)))
			{
				Ball.X = GameConstants.ScreenWidth / 2 - GameConstants.DefaultBallSize / 2;
				Ball.Y = GameConstants.ScreenHeight / 2 - GameConstants.DefaultBallSize / 2;
				Ball.Speed = GameConstants.DefaultInitialBallSpeed;
				HitSound.Play();
			}
			// Paddle - ball collision
			if (CollisionDetector.Overlaps(Ball, PaddleTop) && Ball.Direction.Y < 0
			|| (CollisionDetector.Overlaps(Ball, PaddleBottom) && Ball.Direction.Y > 0))
			{
				Ball.Direction = new Vector2(Ball.Direction.X, -Ball.Direction.Y);
				Ball.Speed *= Ball.BumpSpeedIncreaseFactor;
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			// Start drawing .
			spriteBatch.Begin();
			for (int i = 0; i < SpritesForDrawList.Count; i++)
			{
				SpritesForDrawList.GetElement(i).DrawSpriteOnScreen(spriteBatch);
			}
			// End drawing .
			// Send all gathered details to the graphic card in one batch .
			spriteBatch.End();
			base.Draw(gameTime);
		}

		public class CollisionDetector
		{
			public static bool Overlaps(IPhysicalObject2D a, IPhysicalObject2D b)
			{
				if (a != null && b != null)
				{
					Rectangle r1 = new Rectangle((int) a.X, (int) a.Y, a.Width, a.Height);
					Rectangle r2 = new Rectangle((int) b.X, (int) b.Y, b.Width, b.Height);

					if (r1.Intersects(r2)) return true;
				}
				return false;
			}
		}
	}
}
