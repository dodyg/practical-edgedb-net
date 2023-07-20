CREATE MIGRATION m1yeitm5w5h62kgoqtak3zmcmso25ia62ehev372h6ibdkcbjfy4aq
    ONTO initial
{
  CREATE SCALAR TYPE default::BlogPostStatus EXTENDING enum<Active, Deleted, Pending>;
  CREATE TYPE default::BlogPost {
      CREATE REQUIRED PROPERTY body: std::str;
      CREATE REQUIRED PROPERTY date_created: std::datetime {
          SET default := (std::datetime_current());
      };
      CREATE REQUIRED PROPERTY status: default::BlogPostStatus {
          SET default := (default::BlogPostStatus.Active);
      };
      CREATE PROPERTY title: std::str;
  };
};
