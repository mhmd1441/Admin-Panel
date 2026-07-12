using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPanel.Migrations
{
    /// <summary>
    /// Supabase exposes every table in the "public" schema through its auto-generated
    /// REST API (PostgREST), and by default the "anon" role has access to new tables.
    /// This app never uses that API — all data access goes through the ASP.NET Core
    /// server — so this migration enables Row Level Security on every table (deny-all,
    /// since no policies are defined) and revokes public-schema access from the
    /// anon/authenticated roles entirely. The app itself is unaffected: it connects as
    /// the table owner, which bypasses RLS.
    /// </summary>
    public partial class LockDownSupabasePublicApi : Migration
    {
        private static readonly string[] Tables =
        [
            "AspNetRoleClaims", "AspNetRoles", "AspNetUserClaims", "AspNetUserLogins",
            "AspNetUserRoles", "AspNetUsers", "AspNetUserTokens",
            "Projects", "ProjectContacts", "ProjectDocuments", "ProjectInvoices", "ProjectNotes",
            "__EFMigrationsHistory"
        ];

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var table in Tables)
            {
                migrationBuilder.Sql($"ALTER TABLE \"{table}\" ENABLE ROW LEVEL SECURITY;");
            }

            // Roles only exist on Supabase; guard so the migration also runs on plain Postgres.
            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (SELECT FROM pg_roles WHERE rolname = 'anon') THEN
                        REVOKE ALL ON ALL TABLES IN SCHEMA public FROM anon;
                        REVOKE ALL ON ALL SEQUENCES IN SCHEMA public FROM anon;
                        REVOKE ALL ON ALL FUNCTIONS IN SCHEMA public FROM anon;
                        REVOKE USAGE ON SCHEMA public FROM anon;
                        ALTER DEFAULT PRIVILEGES FOR ROLE postgres IN SCHEMA public REVOKE ALL ON TABLES FROM anon;
                        ALTER DEFAULT PRIVILEGES FOR ROLE postgres IN SCHEMA public REVOKE ALL ON SEQUENCES FROM anon;
                    END IF;

                    IF EXISTS (SELECT FROM pg_roles WHERE rolname = 'authenticated') THEN
                        REVOKE ALL ON ALL TABLES IN SCHEMA public FROM authenticated;
                        REVOKE ALL ON ALL SEQUENCES IN SCHEMA public FROM authenticated;
                        REVOKE ALL ON ALL FUNCTIONS IN SCHEMA public FROM authenticated;
                        REVOKE USAGE ON SCHEMA public FROM authenticated;
                        ALTER DEFAULT PRIVILEGES FOR ROLE postgres IN SCHEMA public REVOKE ALL ON TABLES FROM authenticated;
                        ALTER DEFAULT PRIVILEGES FOR ROLE postgres IN SCHEMA public REVOKE ALL ON SEQUENCES FROM authenticated;
                    END IF;
                END $$;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var table in Tables)
            {
                migrationBuilder.Sql($"ALTER TABLE \"{table}\" DISABLE ROW LEVEL SECURITY;");
            }
        }
    }
}
