﻿namespace FrameworkSDK.MonoGame.Mvc
{
    public class MvcSchemeValidateResult : IMvcSchemeValidateResult
    {
        public bool IsModelExist { get; set; }
        public bool IsViewExist { get; set; }
        public bool IsControllerExist { get; set; }
    }
}
