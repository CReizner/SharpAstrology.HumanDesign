// Shamelessly copied from MathNET: https://github.com/mathnet/mathnet-numerics

namespace SharpAstrology.HumanDesign.Mathematics;

internal static class RootFinder
{
    private static readonly double DoublePrecision = Math.Pow(2.0, -53.0);
    private static readonly double PositiveDoublePrecision = 2.0 * DoublePrecision;
    private static readonly double DefaultDoubleAccuracy = DoublePrecision * 10.0;
    
    public static double FindRoot(Func<double, double> f, double lowerBound, double upperBound, double accuracy = 1E-05, int maxIterations = 100)
    {
        if (!ExpandReduce(f, ref lowerBound, ref upperBound, expansionMaxIterations: maxIterations, reduceSubdivisions: maxIterations * 10))
            throw new Exception("The algorithm has failed, exceeded the number of iterations allowed or there is no root within the provided bounds.");
        double root;
        if (BrentTryFindRoot(f, lowerBound, upperBound, accuracy, maxIterations, out root) || BisectionTryFindRoot(f, lowerBound, upperBound, accuracy, maxIterations, out root))
            return root;
        throw new Exception("The algorithm has failed, exceeded the number of iterations allowed or there is no root within the provided bounds.");
    }
    
    private static bool BrentTryFindRoot( Func<double, double> f, double lowerBound, double upperBound, double accuracy, int maxIterations, out double root)
    {
      if (accuracy <= 0.0)
        throw new ArgumentOutOfRangeException(nameof (accuracy), "Must be greater than zero.");
      double num1 = f(lowerBound);
      double num2 = f(upperBound);
      double num3 = num2;
      double num4 = 0.0;
      double num5 = 0.0;
      root = upperBound;
      double b = double.NaN;
      if (Math.Sign(num1) == Math.Sign(num2))
        return false;
      for (int index = 0; index <= maxIterations; ++index)
      {
        if (Math.Sign(num3) == Math.Sign(num2))
        {
          upperBound = lowerBound;
          num2 = num1;
          num5 = num4 = root - lowerBound;
        }
        if (Math.Abs(num2) < Math.Abs(num3))
        {
          lowerBound = root;
          root = upperBound;
          upperBound = lowerBound;
          num1 = num3;
          num3 = num2;
          num2 = num1;
        }
        double a = PositiveDoublePrecision * Math.Abs(root) + 0.5 * accuracy;
        double num6 = b;
        b = (upperBound - root) / 2.0;
        if (Math.Abs(b) <= a || num3.AlmostEqualNormRelative(0.0, num3, accuracy))
          return true;
        if (b == num6)
          return false;
        if (Math.Abs(num5) >= a && Math.Abs(num1) > Math.Abs(num3))
        {
          double num7 = num3 / num1;
          double num8;
          double num9;
          if (lowerBound.AlmostEqualRelative(upperBound))
          {
            num8 = 2.0 * b * num7;
            num9 = 1.0 - num7;
          }
          else
          {
            double num10 = num1 / num2;
            double num11 = num3 / num2;
            num8 = num7 * (2.0 * b * num10 * (num10 - num11) - (root - lowerBound) * (num11 - 1.0));
            num9 = (num10 - 1.0) * (num11 - 1.0) * (num7 - 1.0);
          }
          if (num8 > 0.0)
            num9 = -num9;
          double num12 = Math.Abs(num8);
          if (2.0 * num12 < Math.Min(3.0 * b * num9 - Math.Abs(a * num9), Math.Abs(num5 * num9)))
          {
            num5 = num4;
            num4 = num12 / num9;
          }
          else
          {
            num4 = b;
            num5 = num4;
          }
        }
        else
        {
          num4 = b;
          num5 = num4;
        }
        lowerBound = root;
        num1 = num3;
        if (Math.Abs(num4) > a)
          root += num4;
        else
          root += Sign(a, b);
        num3 = f(root);
      }
      return false;
    }
    
    private static bool BisectionTryFindRoot( Func<double, double> f, double lowerBound, double upperBound, double accuracy, int maxIterations, out double root)
    {
        if (accuracy <= 0.0)
            throw new ArgumentOutOfRangeException(nameof (accuracy), "Must be greater than zero.");
        if (upperBound < lowerBound)
        {
            double num1 = lowerBound;
            double num2 = upperBound;
            upperBound = num1;
            lowerBound = num2;
        }
        double num3 = f(lowerBound);
        if (Math.Sign(num3) == 0)
        {
            root = lowerBound;
            return true;
        }
        double num4 = f(upperBound);
        if (Math.Sign(num4) == 0)
        {
            root = upperBound;
            return true;
        }
        root = 0.5 * (lowerBound + upperBound);
        if (Math.Sign(num3) == Math.Sign(num4))
            return false;
        for (int index = 0; index <= maxIterations; ++index)
        {
            double num5 = f(root);
            if (upperBound - lowerBound <= 2.0 * accuracy && Math.Abs(num5) <= accuracy)
                return true;
            if (lowerBound == root || upperBound == root)
                return false;
            if (Math.Sign(num5) == Math.Sign(num3))
            {
                lowerBound = root;
                num3 = num5;
            }
            else
            {
                if (Math.Sign(num5) != Math.Sign(num4))
                    return true;
                upperBound = root;
                num4 = num5;
            }
            root = 0.5 * (lowerBound + upperBound);
        }
        return false;
    }
    
    #region Helper Methods
    private static bool ExpandReduce( Func<double, double> f, ref double lowerBound, ref double upperBound, double expansionFactor = 1.6, int expansionMaxIterations = 50, int reduceSubdivisions = 100)
    {
        return Expand(f, ref lowerBound, ref upperBound, expansionFactor, expansionMaxIterations) || Reduce(f, ref lowerBound, ref upperBound, reduceSubdivisions);
    }
    
    private static bool Expand( Func<double, double> f, ref double lowerBound, ref double upperBound, double factor = 1.6, int maxIterations = 50)
    {
        double num1 = lowerBound;
        double num2 = upperBound;
        if (lowerBound >= upperBound)
            throw new ArgumentOutOfRangeException(nameof (upperBound), "xmax must be greater than xmin.");
        double num3 = f(lowerBound);
        double num4 = f(upperBound);
        for (int index = 0; index < maxIterations; ++index)
        {
            if (Math.Sign(num3) != Math.Sign(num4))
                return true;
            if (Math.Abs(num3) < Math.Abs(num4))
            {
                lowerBound += factor * (lowerBound - upperBound);
                num3 = f(lowerBound);
            }
            else
            {
                upperBound += factor * (upperBound - lowerBound);
                num4 = f(upperBound);
            }
        }
        lowerBound = num1;
        upperBound = num2;
        return false;
    }
    
    private static bool Reduce( Func<double, double> f, ref double lowerBound, ref double upperBound, int subdivisions = 1000)
    {
        double num1 = lowerBound;
        double num2 = upperBound;
        if (lowerBound >= upperBound)
            throw new ArgumentOutOfRangeException(nameof (upperBound), "xmax must be greater than xmin.");
        double num3 = f(lowerBound);
        double num4 = f(upperBound);
        if (Math.Sign(num3) != Math.Sign(num4))
            return true;
        double num5 = (upperBound - lowerBound) / (double) subdivisions;
        double num6 = lowerBound;
        int num7 = Math.Sign(num3);
        for (int index = 0; index < subdivisions; ++index)
        {
            double num8 = num6 + num5;
            double d = f(num8);
            if (double.IsInfinity(d))
            {
                num6 = num8;
            }
            else
            {
                if (Math.Sign(d) != num7)
                {
                    lowerBound = num6;
                    upperBound = num8;
                    return true;
                }
                num6 = num8;
            }
        }
        lowerBound = num1;
        upperBound = num2;
        return false;
    }
    
    private static double Sign(double a, double b)
    {
        return b < 0.0 ? (a < 0.0 ? a : -a) : (a < 0.0 ? -a : a);
    }
    
    private static bool AlmostEqualNormRelative(this double a, double b, double diff, double maximumError)
    {
        if (double.IsInfinity(a) || double.IsInfinity(b))
            return a == b;
        if (double.IsNaN(a) || double.IsNaN(b))
            return false;
        if (Math.Abs(a) < DoublePrecision || Math.Abs(b) < DoublePrecision)
            return Math.Abs(diff) < maximumError;
        return a == 0.0 && Math.Abs(b) < maximumError || b == 0.0 && Math.Abs(a) < maximumError || Math.Abs(diff) < maximumError * Math.Max(Math.Abs(a), Math.Abs(b));
    }
    
    private static bool AlmostEqualRelative(this double a, double b)
    {
        return a.AlmostEqualNormRelative(b, a - b, DefaultDoubleAccuracy);
    }
    
    #endregion
}