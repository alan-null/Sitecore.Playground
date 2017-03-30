using Rainbow.Model;
using Unicorn.Predicates;

namespace Sitecore.Playground.Serialization.Predicates
{
    public interface IPresetItemExclusion
    {
        string Description { get; }

        PredicateResult Evaluate(IItemData itemData);
    }
}