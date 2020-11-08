USE `comet.account`;

UPDATE `realm` SET `Name` = 'Storm' WHERE `Name` = 'Comet';

USE `comet.game`;

DROP PROCEDURE IF EXISTS `upgrade_5017_5187`;
DELIMITER //

CREATE PROCEDURE `upgrade_5017_5187`() 
BEGIN

  IF NOT EXISTS(
    SELECT NULL
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE table_name = 'character'
      AND table_schema = 'comet.game'
      AND column_name = 'AncestorClass')
  THEN
    ALTER TABLE `character` 
      ADD COLUMN `AncestorClass` TINYINT(3) UNSIGNED NOT NULL DEFAULT 0 
      AFTER `PreviousClass`;
  END IF;

  IF NOT EXISTS(
    SELECT NULL
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE table_name = 'character'
      AND table_schema = 'comet.game'
      AND column_name = 'EnlightenPoints')
  THEN
    ALTER TABLE `character` 
      ADD COLUMN `EnlightenPoints` TINYINT(3) UNSIGNED NOT NULL DEFAULT 0 
      AFTER `KillPoints`;
  END IF;

  IF NOT EXISTS(
    SELECT NULL
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE table_name = 'character'
      AND table_schema = 'comet.game'
      AND column_name = 'QuizPoints')
  THEN
    ALTER TABLE `character` 
      ADD COLUMN `QuizPoints` TINYINT(3) UNSIGNED NOT NULL DEFAULT 0 
      AFTER `EnlightenPoints`;
  END IF;

  IF NOT EXISTS(
    SELECT NULL
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE table_name = 'character'
      AND table_schema = 'comet.game'
      AND column_name = 'VIPLevel')
  THEN 
    ALTER TABLE `character` 
      ADD COLUMN `VIPLevel` TINYINT(3) UNSIGNED NOT NULL DEFAULT 0 
      AFTER `QuizPoints`;
  END IF;

END //

DELIMITER ;

CALL `upgrade_5017_5187`();
DROP PROCEDURE `upgrade_5017_5187`;
