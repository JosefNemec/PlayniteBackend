namespace Playnite.Backend;

public class AssertException : Exception
{
    public AssertException() : base()
    {
    }

    public AssertException(string message) : base(message)
    {
    }
}

public class TestAssert
{
    public static void IsTrue(bool condition)
    {
        if (!condition)
        {
            throw new AssertException();
        }
    }

    public static void IsTrue(bool condition, string message)
    {
        if (!condition)
        {
            throw new AssertException(message);
        }
    }

    public static void IsFalse(bool condition)
    {
        if (condition)
        {
            throw new AssertException();
        }
    }

    public static void IsFalse(bool condition, string message)
    {
        if (condition)
        {
            throw new AssertException(message);
        }
    }
}
