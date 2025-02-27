namespace comApp.db
{
    public static class SqlQueries
    {
        public const string InsertUser = @"
            INSERT INTO Users (Name, Email, Password) 
            VALUES (@Name, @Email, @Password);
        ";

        public const string SelectUserByEmailAndPassword = @"
            SELECT * FROM Users 
            WHERE Email = @Email AND Password = @Password;
        ";
    }
}
