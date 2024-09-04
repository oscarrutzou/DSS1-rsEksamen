using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace DoctorsDungeon.GameManagement
{

    public class Canvas
    {
        private RenderTarget2D _baseScreen, _highlights, _blurFirstPass, _blurSecondPass;
        private RenderTarget2D _highlightsUI, _blurFirstPassUI, _blurSecondPassUI;
        private readonly GraphicsDevice _graphicsDevice;
        private Rectangle _destinationRectangle;
        Texture2D vignette;
        private enum ShaderEffectNames
        {
            // The name for each effect
            Bloom,
            SingleColor,
        }
        public Canvas(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        private void AddBloom()
        {
            int width = GameWorld.Instance.DisplayWidth;
            int height = GameWorld.Instance.DisplayHeight;

            vignette = new Texture2D(_graphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = Color.Black; // Or any other color you want
            }
            vignette.SetData(data);

            _highlights = new RenderTarget2D(_graphicsDevice, width, height);
            _blurFirstPass = new RenderTarget2D(_graphicsDevice, width, height);
            _blurSecondPass = new RenderTarget2D(_graphicsDevice, width, height);


            _highlightsUI = new RenderTarget2D(_graphicsDevice, width, height);
            _blurFirstPassUI = new RenderTarget2D(_graphicsDevice, width, height);
            _blurSecondPassUI = new RenderTarget2D(_graphicsDevice, width, height);
        }

        /// <summary>
        /// This is so we can set the base game to be a smaller scale and then just upscale everything. 
        /// <para>Does nothing right now</para>
        /// </summary>
        public void SetDestinationRectangle()
        {
            int width = GameWorld.Instance.DisplayWidth;
            int height = GameWorld.Instance.DisplayHeight;

            _baseScreen = new(_graphicsDevice, width, height);

            // Makes a new shader list
            AddBloom();

            var screenSize = _graphicsDevice.PresentationParameters.Bounds;
            float scaleX = (float)screenSize.Width / _baseScreen.Width;
            float scaleY = (float)screenSize.Height / _baseScreen.Height;
            float scale = Math.Min(scaleX, scaleY);

            int newWidth = (int)(_baseScreen.Width * scale);
            int newHeight = (int)(_baseScreen.Height * scale);

            int posX = (screenSize.Width - newWidth) / 2;
            int posY = (screenSize.Height - newHeight) / 2;

            _destinationRectangle = new Rectangle(0, 0, newWidth, newHeight);
        }

        public void Activate()
        {
            _graphicsDevice.SetRenderTarget(_baseScreen);
        }

        private void SetBloomOnWorldScreen(SpriteBatch spriteBatch)
        {
            _graphicsDevice.SetRenderTarget(_highlights);

            spriteBatch.Begin(effect: GlobalTextures.HighlightsEffect);
            spriteBatch.Draw(_baseScreen, Vector2.Zero, Color.White);
            spriteBatch.End();

            _graphicsDevice.SetRenderTarget(_blurFirstPass);

            // Horizontal pass
            GlobalTextures.BlurEffect.CurrentTechnique = GlobalTextures.BlurEffect.Techniques["Vertical"];
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, effect: GlobalTextures.BlurEffect);
            spriteBatch.Draw(_highlights, Vector2.Zero, Color.White);
            spriteBatch.End();

            _graphicsDevice.SetRenderTarget(_blurSecondPass);

            // Vertical pass
            GlobalTextures.BlurEffect.CurrentTechnique = GlobalTextures.BlurEffect.Techniques["Horizontal"];
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, effect: GlobalTextures.BlurEffect);
            spriteBatch.Draw(_blurFirstPass, Vector2.Zero, Color.White);
            spriteBatch.End();
        }
        // Draw the blurred texture on top using a custom blend state
        //BlendState customBlendState = new BlendState
        //{
        //    ColorSourceBlend = Blend.One,
        //    ColorDestinationBlend = Blend.Zero,
        //    AlphaSourceBlend = Blend.One,
        //    AlphaDestinationBlend = Blend.One
        //};

        private void SetBloomOnUIScreen(SpriteBatch spriteBatch)
        {
            _graphicsDevice.SetRenderTarget(_highlightsUI);
            _graphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(effect: GlobalTextures.HighlightsEffect);
            
            
            //spriteBatch.Draw(GameWorld.Instance.UIRenderTarget, Vector2.Zero, Color.White);


            spriteBatch.End();

            _graphicsDevice.SetRenderTarget(_blurFirstPassUI);
            _graphicsDevice.Clear(Color.Transparent);

            // Horizontal pass
            GlobalTextures.BlurEffect.CurrentTechnique = GlobalTextures.BlurEffect.Techniques["Vertical"];
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, effect: GlobalTextures.BlurEffect);
            spriteBatch.Draw(_highlightsUI, Vector2.Zero, Color.White);
            spriteBatch.End();

            _graphicsDevice.SetRenderTarget(_blurSecondPassUI);
            _graphicsDevice.Clear(Color.Transparent);

            // Vertical pass
            GlobalTextures.BlurEffect.CurrentTechnique = GlobalTextures.BlurEffect.Techniques["Horizontal"];
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, effect: GlobalTextures.BlurEffect);
            spriteBatch.Draw(_blurFirstPassUI, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Texture2D gaussinBlurTex = _baseScreen;

            SetBloomOnWorldScreen(spriteBatch);

            //SetBloomOnUIScreen(spriteBatch);

            // Reset the render target
            _graphicsDevice.SetRenderTarget(null);

            // Clear the screen
            _graphicsDevice.Clear(Color.Transparent);

            if (GameWorld.Instance.SingleColorEffect)
                DrawBaseScreen(spriteBatch, GlobalTextures.SingleColorEffect); // Need to make this effect also contain vignette
            else
                DrawBaseScreen(spriteBatch, GlobalTextures.VignetteEffect);


            // Draw the rest of the effects (All are going to be having chromatic aberration on them 
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive, effect: GameWorld.Instance.SingleColorEffect ? null : GlobalTextures.ChromaticAberrationEffect);

            //spriteBatch.Draw(_baseScreen, Vector2.Zero, Color.White);
            spriteBatch.Draw(_blurSecondPass, Vector2.Zero, Color.White);
            spriteBatch.End();

            //spriteBatch.Begin(SpriteSortMode.FrontToBack);
            //spriteBatch.Draw(GameWorld.Instance.UIRenderTarget, Vector2.Zero, Color.White);
            ////spriteBatch.Draw(_blurSecondPassUI, Vector2.Zero, Color.White);
            //spriteBatch.End();
        }

        private void DrawBaseScreen(SpriteBatch spriteBatch, Effect effect)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive, effect: effect);
            spriteBatch.Draw(_baseScreen, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

    }

    //public class ShaderEffectData
    //{
    //    public RenderTarget2D RenderTarget { get; set; }
    //    public Effect ShaderEffect {  get; set; }

    //    public ShaderEffectData(GraphicsDevice graphicsDevice, int width, int height, Effect effect)
    //    {
    //        RenderTarget = new RenderTarget2D(graphicsDevice, width, height);
    //        ShaderEffect = effect;
    //    }

    //    //if (ShaderEffects.ContainsKey(ShaderEffectNames.Bloom)) ShaderEffects.Remove(ShaderEffectNames.Bloom);

    //    //ShaderEffects.Add(ShaderEffectNames.Bloom, new List<ShaderEffectData>()
    //    //{
    //    //    { new ShaderEffectData(_graphicsDevice, width, height, GlobalTextures.HighlightsEffect)  },
    //    //    { new ShaderEffectData(_graphicsDevice, width, height, GlobalTextures.BlurEffect)  }, // GaussianBlur
    //    //});

    //    // Everthing drawn before this, will be used for the effect

    //    //Texture2D finnishedScene = _baseScreen;

    //    //foreach (var shaderEffectPair in ShaderEffects)
    //    //{
    //    //    Texture2D baseScene = _baseScreen;

    //    //    ShaderEffectNames shaderEffectName = shaderEffectPair.Key;

    //    //    // Iterate over the inner dictionary (RenderTarget2D and Effect pairs)
    //    //    foreach (ShaderEffectData shaderEffectData in shaderEffectPair.Value)
    //    //    {
    //    //        RenderTarget2D renderTarget = shaderEffectData.RenderTarget;
    //    //        Effect shaderEFfect = shaderEffectData.ShaderEffect;

    //    //        _graphicsDevice.SetRenderTarget(renderTarget);

    //    //        spriteBatch.Begin(effect: shaderEFfect);
    //    //        spriteBatch.Draw(baseScene, Vector2.Zero, Color.White);
    //    //        spriteBatch.End();

    //    //        baseScene = renderTarget;
    //    //    }
    //    //}
    //    //foreach (var shaderEffectPair in ShaderEffects)
    //    //{
    //    //    ShaderEffectNames shaderEffectName = shaderEffectPair.Key;

    //    //    // The last has the final effect
    //    //    RenderTarget2D shaderTarget = shaderEffectPair.Value.Last().RenderTarget;
    //    //    spriteBatch.Draw(shaderTarget, Vector2.Zero, Color.White); // Draw last of the effect
    //    //}
    //}
}
