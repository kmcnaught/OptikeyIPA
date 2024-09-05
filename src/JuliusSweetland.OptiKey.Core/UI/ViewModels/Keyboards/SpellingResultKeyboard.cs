// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using System;
using JuliusSweetland.OptiKey.UI.ViewModels.Keyboards.Base;
using System.Collections.Generic;
using JuliusSweetland.OptiKey.Services;
using log4net;
using JuliusSweetland.OptiKey.Services.Suggestions.Phonemics;

namespace JuliusSweetland.OptiKey.UI.ViewModels.Keyboards
{
    public class SpellingResultKeyboard : DynamicKeyboard
    {
        public SpellingResultKeyboard(Action backAction,
            IKeyStateService keyStateService,
            LevenshteinResult result,
            Dictionary<Models.KeyValue, Enums.KeyDownStates> overrideKeyStates = null) : base(backAction, keyStateService, "", overrideKeyStates)
        {
            this.result = result;
        }

        private LevenshteinResult result;
        public LevenshteinResult Result
        {
            get { return result; }
        }
    }
}
