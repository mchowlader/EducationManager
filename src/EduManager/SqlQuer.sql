protected override void Up(MigrationBuilder migrationBuilder)
{
    // ১. tenant_config table
    migrationBuilder.Sql("""
        CREATE TABLE tenant_config (
            key VARCHAR(50) PRIMARY KEY,
            value VARCHAR(200) NOT NULL
        );
    """);

    // ২. Sequences
    migrationBuilder.Sql("CREATE SEQUENCE user_code_seq START 1 INCREMENT 1;");
    migrationBuilder.Sql("CREATE SEQUENCE student_code_seq START 1 INCREMENT 1;");
    migrationBuilder.Sql("CREATE SEQUENCE teacher_code_seq START 1 INCREMENT 1;");

    // ৩. User Code Trigger
    migrationBuilder.Sql("""
        CREATE OR REPLACE FUNCTION generate_user_code()
        RETURNS TRIGGER AS $$
        DECLARE
            slug_code TEXT;
            seq_num BIGINT;
        BEGIN
            SELECT value INTO slug_code FROM tenant_config WHERE key = 'slug_code';
            seq_num := nextval('user_code_seq');
            NEW."UserCode" := slug_code || 'U' || LPAD(seq_num::TEXT, 5, '0');
            RETURN NEW;
        END;
        $$ LANGUAGE plpgsql;

        CREATE TRIGGER trg_generate_user_code
            BEFORE INSERT ON "Users"
            FOR EACH ROW
            WHEN (NEW."UserCode" IS NULL OR NEW."UserCode" = '')
            EXECUTE FUNCTION generate_user_code();
    """);

    // ৪. Student Code Trigger
    migrationBuilder.Sql("""
        CREATE OR REPLACE FUNCTION generate_student_code()
        RETURNS TRIGGER AS $$
        DECLARE
            slug_code TEXT;
            seq_num BIGINT;
        BEGIN
            SELECT value INTO slug_code FROM tenant_config WHERE key = 'slug_code';
            seq_num := nextval('student_code_seq');
            NEW."StudentCode" := slug_code || 'S' || LPAD(seq_num::TEXT, 5, '0');
            RETURN NEW;
        END;
        $$ LANGUAGE plpgsql;

        CREATE TRIGGER trg_generate_student_code
            BEFORE INSERT ON "Students"
            FOR EACH ROW
            WHEN (NEW."StudentCode" IS NULL OR NEW."StudentCode" = '')
            EXECUTE FUNCTION generate_student_code();
    """);

    // ৫. Teacher Code Trigger
    migrationBuilder.Sql("""
        CREATE OR REPLACE FUNCTION generate_teacher_code()
        RETURNS TRIGGER AS $$
        DECLARE
            slug_code TEXT;
            seq_num BIGINT;
        BEGIN
            SELECT value INTO slug_code FROM tenant_config WHERE key = 'slug_code';
            seq_num := nextval('teacher_code_seq');
            NEW."TeacherCode" := slug_code || 'T' || LPAD(seq_num::TEXT, 5, '0');
            RETURN NEW;
        END;
        $$ LANGUAGE plpgsql;

        CREATE TRIGGER trg_generate_teacher_code
            BEFORE INSERT ON "Teachers"
            FOR EACH ROW
            WHEN (NEW."TeacherCode" IS NULL OR NEW."TeacherCode" = '')
            EXECUTE FUNCTION generate_teacher_code();
    """);
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_generate_user_code ON \"Users\";");
    migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_generate_student_code ON \"Students\";");
    migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_generate_teacher_code ON \"Teachers\";");
    migrationBuilder.Sql("DROP FUNCTION IF EXISTS generate_user_code;");
    migrationBuilder.Sql("DROP FUNCTION IF EXISTS generate_student_code;");
    migrationBuilder.Sql("DROP FUNCTION IF EXISTS generate_teacher_code;");
    migrationBuilder.Sql("DROP SEQUENCE IF EXISTS user_code_seq;");
    migrationBuilder.Sql("DROP SEQUENCE IF EXISTS student_code_seq;");
    migrationBuilder.Sql("DROP SEQUENCE IF EXISTS teacher_code_seq;");
    migrationBuilder.Sql("DROP TABLE IF EXISTS tenant_config;");
}


-----------------------------------------------------------------------------
INSERT INTO "AdminUsers" 
("FullName", "Email", "PasswordHash", "IsActive", "Role", "CreatedAt", "IsDelete", "CreateBy", "UpdateBy")
VALUES 
(
    'Super Admin',
    'admin@edumanager.com',
    '$2y$10$2Quj4Z1T/z3n9BTPsZfcSu7o1.w8/5RW3uCSxshNNI8aYaHQHY8MK',
    true,
    1,
    NOW(),
    false,
    0,
    0
);