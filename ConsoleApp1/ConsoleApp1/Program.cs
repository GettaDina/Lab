using System.Globalization;
using System.Net;
using System.Text;
using MySqlConnector;
using System.IO.Compression;
using System;

class Program
{
    public static async Task Main()
    {
        using (HttpClient client = new HttpClient())
        {
            using (HttpResponseMessage response = await client.GetAsync(
                       "https://drive.google.com/u/0/uc?id=1sEJfhld56qAXJwpIRP3o-AfGv_beq9jN&export=download"))
            using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
            {
                using (FileStream decompressedFileStream = File.Create("257.csv"))
                using (GZipStream decompressionStream = new GZipStream(streamToReadFrom, CompressionMode.Decompress))
                    decompressionStream.CopyTo(decompressedFileStream);
            }
        }

        var data = new StringBuilder(InsertData);

        var builder = new MySqlConnectionStringBuilder
        {
            Server = "gps.antelis.by",
            UserID = "antelis",
            Password = "jgznm'njktnj"
        };

        await using var conn = new MySqlConnection(builder.ToString());
        conn.Open();
        await using var cmd = conn.CreateCommand();

        cmd.CommandText = CreateTable;
        cmd.ExecuteNonQuery();

        using var sr = File.OpenText("257.csv");
        string? line;

        while ((line = sr.ReadLine()) != null)
        {
            int i = 0;
            if (line[i++] != 'G' ||
                line[i++] != 'S' ||
                line[i++] != 'M' ||
                line[i++] != ',' ||
                !TryParseInt(line, ref i, out var mcc) ||
                !TryParseInt(line, ref i, out var mnc) ||
                !TryParseInt(line, ref i, out var lac) ||
                !TryParseInt(line, ref i, out var cid) ||
                !TryParseDouble(line, ref i, out var lat) ||
                !TryParseDouble(line, ref i, out var lng))
            {
                continue;
            }

            data.AppendLine().Append($"({mcc}),({mnc}),({lac}),({cid}),({lat}),({lng}),");
            if (data.Length > 512 * 1024)
            {
                data.Length--;
                cmd.CommandText = data.ToString();
                cmd.ExecuteNonQuery();
                data.Length = InsertData.Length;
            }
        }

        if (data.Length > InsertData.Length)
        {
            data.Length--;
            cmd.CommandText = data.ToString();
            cmd.ExecuteNonQuery();
            data.Length = InsertData.Length;
        }
    }

    private static bool TryParseInt(string line, ref int curr, out int value)
    {
        int next = line.IndexOf(',', curr);
        if (!int.TryParse(line.AsSpan()[curr..next], out value))
            return false;
        curr = next + 1;
        return true;
    }
    private static bool TryParseDouble(string line, ref int curr, out double value)
    {
        int next = line.IndexOf(',', curr);
        if (!double.TryParse(line.AsSpan()[curr..next], Style, Format, out value))
            return false;
        curr = next + 1;
        return true;
    }

    private static readonly NumberFormatInfo Format = NumberFormatInfo.InvariantInfo;
    private static readonly NumberStyles Style = NumberStyles.Number;

    private const string CreateTable = @"CREATE TABLE IF NOT EXISTS `gps2`.`gsm` (
`id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
`mcc` INT UNSIGNED NOT NULL,
`mnc` INT UNSIGNED NOT NULL,
`lac` INT UNSIGNED NOT NULL,
`cid` INT UNSIGNED NOT NULL,
`lat` DOUBLE NOT NULL,
`lng` DOUBLE NOT NULL,
PRIMARY KEY (`id`),
UNIQUE INDEX `UNIQUE` (`mcc` ASC, `mnc` ASC, `lac` ASC, `cid` ASC));
";

    private const string InsertData = @"INSERT IGNORE INTO `gps2`.`gsm` (`mcc`,`mnc`,`lac`,`cid`,`lat`,`lng`) VALUES";

}