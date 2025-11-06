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
}

class Programm
{
    static void Main(string[] args)
    {
        double[] coeffs = { 1.0, 0.0, 2.0 };
        Polynomial p = new Polynomial(coeffs); // 1 + 2x^2

        Console.WriteLine(p);
    }
}