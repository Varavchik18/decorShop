INSERT INTO [Product.Category].[Sections_tb] (Name)
VALUES
('Living Room'),
('Bedroom'),
('Kitchen'),
('Bathroom');

INSERT INTO [Product.Category].[Categories_tb] (Name, SectionId)
VALUES
('Sofas', 12),        -- Living Room
('Beds', 13),         -- Bedroom
('Dining Tables', 14),-- Kitchen
('Showers', 15);      -- Bathroom

INSERT INTO [Product.Category].[Subcategories_tb] (Name, IconUrl, CategoryId)
VALUES
('Leather Sofas', 'url_to_icon1', 24),      -- Sofas
('Fabric Sofas', 'url_to_icon2', 24),       -- Sofas
('King Size Beds', 'url_to_icon3', 25),     -- Beds
('Queen Size Beds', 'url_to_icon4', 25),    -- Beds
('Round Dining Tables', 'url_to_icon5', 26),-- Dining Tables
('Rectangular Dining Tables', 'url_to_icon6', 26),-- Dining Tables
('Shower Enclosures', 'url_to_icon7', 27),  -- Showers
('Shower Panels', 'url_to_icon8', 27);      -- Showers
