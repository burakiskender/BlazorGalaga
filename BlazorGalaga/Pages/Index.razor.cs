﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using BlazorGalaga.Models;
using BlazorGalaga.Services;
using BlazorGalaga.Static;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorGalaga.Pages
{
    public partial class Index: ComponentBase
    {
        private Canvas2DContext ctx;

        protected BECanvasComponent _canvasReference;
        protected ElementReference spriteSheet;

        private static Animation shipAnimation;

        [Inject]
        public BezierCurveService bezierCurveService { get; set; }
        [Inject]
        public AnimationService animationService { get; set; }
        [Inject]
        public SpriteService spriteService { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

            ctx = await _canvasReference.CreateCanvas2DAsync();

            spriteService.CanvasCtx = ctx;
            spriteService.SpriteSheet = spriteSheet;

            animationService.CanvasCtx = ctx;

            animationService.InitAnimations();
            //animationService.ComputePathPoints();

            shipAnimation = animationService.Animations.FirstOrDefault(
                a=>a.Animatables.Any(
                        b=>b.Sprite.SpriteType == Sprite.SpriteTypes.Ship
                    )
                );

            await JsRuntime.InvokeAsync<object>("initFromBlazor", DotNetObjectReference.Create(this));

        }

        bool exit;

        [JSInvokable]
        public async ValueTask GameLoop()
        {
            if (exit) return;

            await animationService.ResetCanvas();

            animationService.ComputePathPoints();
            exit = true;

            //KeyBoardHelper.ControlShip(shipAnimation);

            //foreach (Animation a in animationService.Animations)
            //{
            //    animationService.Animate(a);
            //    animationService.Draw(a);
            //}
        }

    }
}
