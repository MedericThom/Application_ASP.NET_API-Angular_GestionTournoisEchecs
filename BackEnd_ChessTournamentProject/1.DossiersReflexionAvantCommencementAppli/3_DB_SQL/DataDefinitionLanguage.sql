-- ORDRE DES TABLES A ECRIRE --

-- TABLES AVEC AUCUNE DEPENDANCE :
-- - TABLE Tournament
-- - TABLE Category
-- - TABLE ChessClub

-- TABLES AVEC 1 DEPENDANCE :
-- - TABLE Player

-- TABLES AVEC 2 DEPENDANCES :
-- - TABLE Registration (Tourmament - Player)
-- - TABLE TournamentCategory (Tourmament - Category)
-- - TABLE Match (Match contient => Tournament - WhitePlayer - BlackPLayer)


CREATE TABLE ChessClub (
	Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	NameChessClub NVARCHAR(50) NOT NULL,
);

CREATE TABLE Player (
	Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Pseudo NVARCHAR(50) NOT NULL UNIQUE,
	Email NVARCHAR(50) NOT NULL UNIQUE,
	Pwd NVARCHAR(MAX) NOT NULL,
	BirthDAte DATE NOT NULL,
	Gender NVARCHAR(10) NOT NULL,
	Elo INT NOT NULL DEFAULT 1200,
	ScorePlayer INT NOT NULL,
	ChessClub_Id INT,

	FOREIGN KEY (ChessClub_Id) REFERENCES ChessClub(Id)
);


CREATE TABLE Category ( 
   Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
   NameCategory NVARCHAR(50) NOT NULL,
   MinAge INT NOT NULL CHECK (MinAge >= 18),
   MaxAge INT NOT NULL CHECK (MaxAge <= 110)
);

CREATE TABLE Match (
	Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    WhitePLayer_Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	BlackPlayer_Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	RoundNumber INT NOT NULL,
	Result INT NULL
);

CREATE TABLE Tournament (
	Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	NameTournament NVARCHAR(50) NOT NULL,
	Place NVARCHAR(50) NOT NULL,
	MinNbPlayer INT NOT NULL,
	MaxNbPLayer INT NOT NULL,
    CHECK (MinNbPlayer <= MaxNbPlayer),
	MinElo INT,
	MaxElo INT,
    CHECK (MinElo <= MaxElo),
	StatusTournament NVARCHAR(50) NOT NULL DEFAULT 'En attente de joueurs',
	CurrentRound INT NOT NULL DEFAULT 0,
	WomenOnly BIT NOT NULL DEFAULT 0,
	RegistrationDeadline DATETIME NOT NULL,
	CreationDate DATETIME NOT NULL DEFAULT GETDATE(),
	UpdateDate DATETIME NOT NULL DEFAULT GETDATE(),
	Match_Id INT,

	FOREIGN KEY (Match_Id) REFERENCES Match(Id)
);


--TABLES INTERMEDIAIRES MANY-TO-MANY
CREATE TABLE Registration
(
   Id INT PRIMARY KEY IDENTITY (1,1),
   Player_Id INT NOT NULL,
   Tournament_Id INT NOT NULL,
   Wins INT DEFAULT 0,
   Losses INT DEFAULT 0,
   Draws INT DEFAULT 0,
   Score DECIMAL(4,1) DEFAULT 0,
   MatchesPlayed INT DEFAULT 0,
   RegistrationDate DATETIME DEFAULT GETDATE(),

   FOREIGN KEY (Player_Id)     REFERENCES PLayer(Id),
   FOREIGN KEY (Tournament_Id) REFERENCES Tournament(Id),

   UNIQUE (Player_Id, Tournament_Id)
);


CREATE TABLE PlayerMatch
(
   Player_Id INT NOT NULL,
   WhitePlayer_Id INT NOT NULL,
   BlackPlayer_Id INT NOT NULL,
   Match_Id INT NOT NULL,

   PRIMARY KEY (Player_id,Match_Id),

   FOREIGN KEY (Player_Id) REFERENCES Player(Id),
   FOREIGN KEY (Match_Id) REFERENCES Match(Id),
   FOREIGN KEY (WhitePlayer_Id) REFERENCES Match(WhitePLayer_Id),
   FOREIGN KEY (BlackPlayer_Id) REFERENCES 
);


CREATE TABLE TournamentCategory 
(
   Tournament_Id INT NOT NULL,
   Category_Id INT NOT NULL,

   PRIMARY KEY (Tournament_Id, Category_Id),

   FOREIGN KEY (Tournament_Id) REFERENCES Tournament(id),
   FOREIGN KEY (Category_Id) REFERENCES Category(Id)
);

--TABLES INTERMEDIAIRES MANY-TO-MANY:
-- - PLayerTournament OK
-- - PlayerMatch OK
-- - MatchTournament OK
-- - TournamentCategory OK


--RELATION One-To-Many:
-- - ChessClub_Id dans Player OK
-- - Match_id dans Tournament OK
