namespace DatingApp.Extensions
{
    public static class CalculateAge
    {
        public static int CalculateAgee(this DateOnly datob)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            
            var age= today.Year - datob.Year;

            if (datob > today.AddYears(-age)) age--;

            return age;
        }
    }
}
