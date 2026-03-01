-- Create Users table if it doesn't exist
CREATE TABLE IF NOT EXISTS "Users" (
    "Id"           SERIAL PRIMARY KEY,
    "Username"     VARCHAR(50)  NOT NULL UNIQUE,
    "PasswordHash" TEXT         NOT NULL
);

-- Create Notes table if it doesn't exist
CREATE TABLE IF NOT EXISTS "Notes" (
    "Id"        SERIAL PRIMARY KEY,
    "Title"     VARCHAR(255) NOT NULL,
    "Content"   TEXT         NULL,
    "CreatedAt" TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    "UserId"    INT          NOT NULL,
    CONSTRAINT "FK_Notes_Users" FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE
);
