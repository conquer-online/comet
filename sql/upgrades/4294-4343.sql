USE `comet.game`;

DROP PROCEDURE IF EXISTS `upgrade_4294_4343`;
DELIMITER //

CREATE PROCEDURE `upgrade_4294_4343`() 
BEGIN

  IF NOT EXISTS(
    SELECT NULL
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE table_name = 'character'
      AND table_schema = 'comet.game'
      AND column_name = 'Jewels')
  THEN
    ALTER TABLE `character` 
      ADD COLUMN `Jewels` INT(10) UNSIGNED NOT NULL DEFAULT 0 
      AFTER `Silver`;
  END IF;

END //

DELIMITER ;

CALL `upgrade_4294_4343`();
DROP PROCEDURE `upgrade_4294_4343`;
