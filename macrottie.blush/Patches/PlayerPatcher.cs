using System.Reflection.Metadata;
using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;

namespace macrottie.blush.Patches;

public class PlayerPatcher : IScriptMod
{
    public bool ShouldRun(string path) => path == "res://Scenes/Entities/Player/player.gdc";

    public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens)
    {
        var beginningWaiter = new MultiTokenWaiter([
            t => t.Type is TokenType.PrExtends,
            t => t.Type is TokenType.Identifier
        ]);

        var spoofAnimationDataWaiter = new MultiTokenWaiter([
            t => t.Type is TokenType.OpAssign,
            t => t is IdentifierToken { Name: "drunk_tier" }
        ]);

        var drunkBubblesWaiter = new MultiTokenWaiter(
        [
            t => t is IdentifierToken { Name: "animation_data" },
            t => t.Type is TokenType.BracketOpen,
            t => t.Type is TokenType.Constant,
            t => t.Type is TokenType.BracketClose,
            t => t.Type is TokenType.OpGreater,
            t => t.Type is TokenType.Constant
        ]);

        foreach (var token in tokens)
        {
            if (beginningWaiter.Check(token))
            {
                yield return token;
                yield return new Token(TokenType.Newline);
                yield return new Token(TokenType.PrVar);
                yield return new IdentifierToken("spoof_blush");
                yield return new Token(TokenType.OpAssign);
                yield return new ConstantToken(new BoolVariant(false));
            } else if (spoofAnimationDataWaiter.Check(token))
            {
                yield return token;
                yield return new Token(TokenType.Newline, 1);
                yield return new Token(TokenType.CfIf);
                yield return new IdentifierToken("spoof_blush");
                yield return new Token(TokenType.OpAnd);
                yield return new IdentifierToken("drunk_tier");
                yield return new Token(TokenType.OpLessEqual);
                yield return new ConstantToken(new IntVariant(1));
                yield return new Token(TokenType.Colon);
                yield return new IdentifierToken("animation_data");
                yield return new Token(TokenType.BracketOpen);
                yield return new ConstantToken(new StringVariant("drunk_tier"));
                yield return new Token(TokenType.BracketClose);
                yield return new Token(TokenType.OpAssign);
                yield return new ConstantToken(new IntVariant(10));
            } else if (drunkBubblesWaiter.Check(token))
            {
                yield return token;
                yield return new Token(TokenType.OpAnd);
                yield return new IdentifierToken("animation_data");
                yield return new Token(TokenType.BracketOpen);
                yield return new ConstantToken(new StringVariant("drunk_tier"));
                yield return new Token(TokenType.BracketClose);
                yield return new Token(TokenType.OpLess);
                yield return new ConstantToken(new IntVariant(5));
            }
            else
            {
                yield return token;
            }

            
        }
    }
}