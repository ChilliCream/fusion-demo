namespace Demo.Accounts.Types;

[ObjectType<User>]
public static partial class UserNode
{
    [Shareable]
    public static string GetName([Parent] User user)
        => user.Name!;
}