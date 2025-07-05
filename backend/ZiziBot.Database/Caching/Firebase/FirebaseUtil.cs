using Firebase.Database.Query;

namespace ZiziBot.Database.Caching.Firebase;

internal static class FirebaseUtil
{
    public static ChildQuery ChildTree(this ChildQuery childQuery, string key)
    {
        var trees = key.Split("_");

        foreach (var tree in trees)
        {
            childQuery = childQuery.Child(tree);
        }

        return childQuery;
    }
}