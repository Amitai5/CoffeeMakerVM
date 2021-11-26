public class Program {
    public static int main(String[] args) {
        int fib = Fibonacci(5);
        return Factorial(fib);
    }

    public static int Factorial(int num) {
        if (num <= 1) {
            return 1;
        }
        return num * Factorial(num - 1);
    }

    public static int Fibonacci(int num) {
        if (num <= 0) {
            return 0;
        } else if (num == 1) {
            return 1;
        }
        return Fibonacci(num - 1) + Fibonacci(num - 2);
    }
}