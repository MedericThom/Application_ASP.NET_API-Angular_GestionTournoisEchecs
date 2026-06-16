-- Migration : ajout de MaxRounds dans Tournament
-- DEFAULT 3 necessaire pour les lignes existantes (colonne NOT NULL)
ALTER TABLE Tournament
ADD MaxRounds INT NOT NULL DEFAULT 3;
