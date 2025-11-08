using System;
class Polynomial
{
    private int degree;
    private double[] coeffs;
    public Polynomial()
    {
        degree = 0;
        coeffs = new double[1] { 0.0 };
    }
    public Polynomial(double[] new_coeffs)
    {
        degree = new_coeffs.Length - 1;
        coeffs = (double[])new_coeffs.Clone();
    }
    public int Degree
    {
        get { return degree; }
    }
    public double[] Coeffs
    {
        get { return (double[])coeffs.Clone(); }
    }
    public override string ToString()
    {
        string ans = "";
        for (int i = degree; i >= 0; i--)
        {
            double coef = coeffs[i];
            if (coef == 0) continue;
            if (ans != "" && coef > 0)
                ans += " + ";
            if (coef < 0)
                ans += " - ";
            double coef_2 = Math.Abs(coef);
            if (coef_2 != 1 || i == 0)
                ans += coef_2.ToString();
            if (i > 1)
                ans += $"x^{i}";
            else if (i == 1)
                ans += "x";
        }
        if (ans == "")
            return "0";
        return ans;
    }
    public static Polynomial operator +(Polynomial obj1, Polynomial obj2)
    {
        double[] m1 = obj1.coeffs;
        double[] m2 = obj2.coeffs;
        int d1 = obj1.degree;
        int d2 = obj2.degree;
        int maxDegree = Math.Max(d1, d2);
        double[] resultCoeffs = new double[maxDegree + 1];
        for (int i = 0; i <= maxDegree; i++)
        {
            double coef1 = (i <= d1) ? m1[i] : 0;
            double coef2 = (i <= d2) ? m2[i] : 0;
            resultCoeffs[i] = coef1 + coef2;
        }
        return new Polynomial(resultCoeffs);
    }
    public static Polynomial operator *(Polynomial obj1, double k)
    {
        double[] resultCoeffs = new double[obj1.degree + 1];
        for (int i = 0; i <= obj1.degree; i++)
        {
            resultCoeffs[i] = obj1.coeffs[i] * k;
        }
        return new Polynomial(resultCoeffs);
    }
}
class Program
{
    static void Main(string[] args)
    {
        double[] coeffs = { 1.0, 0.0, 2.0 };
        Polynomial p = new Polynomial(coeffs);
        Console.WriteLine(p);
    }
}