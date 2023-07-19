CREATE MIGRATION m1ue3l4gx32yrphbuatm6rci6fyuerah4w7arlenmib6zng4jo34ua
    ONTO initial
{
  CREATE TYPE default::Message {
      CREATE REQUIRED PROPERTY content: std::str;
  };
};
