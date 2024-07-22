// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using System;
using JuliusSweetland.OptiKey.UI.ViewModels.Keyboards.Base;
using System.Collections.Generic;
using JuliusSweetland.OptiKey.Services;
using log4net;

namespace JuliusSweetland.OptiKey.UI.ViewModels.Keyboards
{
    public class QuizKeyboard : DynamicKeyboard
    {
        public QuizKeyboard(Action backAction,
            IKeyStateService keyStateService,
            string link,
            Dictionary<Models.KeyValue, Enums.KeyDownStates> overrideKeyStates = null) : base(backAction, keyStateService, link, overrideKeyStates)
        {
        }
    }
}
