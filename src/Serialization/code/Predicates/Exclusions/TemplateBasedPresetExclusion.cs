using System;
using Rainbow.Model;
using Unicorn.Predicates;

namespace Sitecore.Playground.Serialization.Predicates.Exclusions
{
    public class TemplateBasedPresetExclusion : IPresetItemExclusion
    {
        private readonly string _excludeChildrenOfTemplate;

        public TemplateBasedPresetExclusion(string getExpectedAttribute, CustomPresetTreeRoot root)
        {
            _excludeChildrenOfTemplate = getExpectedAttribute;
        }

        public string Description => "Exclueds items based on ItemData' template id";

        public PredicateResult Evaluate(IItemData itemData)
        {
            if (itemData.TemplateId.Equals(new Guid(_excludeChildrenOfTemplate)))
            {
                return new PredicateResult(false);
            }
            return new PredicateResult(true);
        }
    }
}