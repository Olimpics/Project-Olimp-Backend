-- RBAC enterprise upgrade (steps 3-12), MariaDB/MySQL compatible.
-- Run manually on the target database before enabling multi-role inheritance in production.

ALTER TABLE `Role`
    ADD COLUMN IF NOT EXISTS `permissions_mask` BIGINT NOT NULL DEFAULT 0,
    ADD COLUMN IF NOT EXISTS `parent_role_id` INT NULL,
    ADD CONSTRAINT `Role_ParentRole`
        FOREIGN KEY (`parent_role_id`) REFERENCES `Role`(`idRole`) ON DELETE SET NULL;

ALTER TABLE `Permissions`
    ADD COLUMN IF NOT EXISTS `bit_index` INT NOT NULL DEFAULT 0;

CREATE TABLE IF NOT EXISTS `user_roles` (
    `user_id` INT NOT NULL,
    `role_id` INT NOT NULL,
    PRIMARY KEY (`user_id`, `role_id`),
    CONSTRAINT `user_roles_users_fk` FOREIGN KEY (`user_id`) REFERENCES `Users`(`idUsers`) ON DELETE CASCADE,
    CONSTRAINT `user_roles_roles_fk` FOREIGN KEY (`role_id`) REFERENCES `Role`(`idRole`) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS `idx_user_roles_user` ON `user_roles` (`user_id`);
CREATE INDEX IF NOT EXISTS `idx_user_roles_role` ON `user_roles` (`role_id`);

-- Role mask recalculation (step 3)
UPDATE `Role` r
SET r.permissions_mask = (
    SELECT COALESCE(BIT_OR(1 << p.bit_index), 0)
    FROM `BindRolePermission` rp
    JOIN `Permissions` p ON p.idPermissions = rp.PermissionId
    WHERE rp.RoleId = r.idRole
);
