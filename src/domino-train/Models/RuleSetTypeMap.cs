using DominoTrain.Enumerations;
using DominoTrain.Interfaces;
using DominoTrain.Models.Rules;

namespace DominoTrain.Models;

public static class RuleSetTypeMap
{
    public static Dictionary<RuleSetType, ISetRules> RuleSets
        => Enum.GetValues(enumType: typeof(RuleSetType))
            .Cast<RuleSetType>()
            .ToDictionary<RuleSetType, RuleSetType, ISetRules>(keySelector: ruleSetType => ruleSetType,
                elementSelector: GetRules);

    public static SetRules GetRules(RuleSetType ruleSetType)
    {
        switch (ruleSetType)
        {
            case RuleSetType.Default:
                return new DefaultSetRules();
            case RuleSetType.Alternate:
                return new AlternateSetRules();
            case RuleSetType.CommunityCenter:
                return new CommunityCenterSetRules();
            default:
                throw new Exception(message: "Unknown rule set type");
        }
    }

    public static RuleSetType GetRuleSetType(ISetRules setRules)
    {
        return setRules.RuleSetType;
    }
}