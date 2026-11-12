namespace MiniVentas.Data
{
    public static class CodigosErrorSql
    {
        // Registro duplicado en restricción PRIMARY KEY o UNIQUE (CONSTRAINT UQ_...)
        public const int RestriccionUnica = 2627;

        // Registro duplicado en un índice único (CREATE UNIQUE INDEX)
        public const int IndiceUnico = 2601;
    }
}
