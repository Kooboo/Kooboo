﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Commerce.MatchRule.ConditionDefines.Product
{
    public class TypeCondition : ConditionDefineBase<TargetModels.Product>
    {
        public override string Name => "ProductType";

        public override ConditionValueType ValueType => ConditionValueType.ProductTypeId;

        protected override object GetPropertyValue(TargetModels.Product model)
        {
            return model.TypeId;
        }
    }
}