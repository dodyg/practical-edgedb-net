module default {
    scalar type BlogPostStatus extending enum<Active, Deleted, Pending>;
    type BlogPost {
        title: str;
        required body: str;
        required date_created: datetime{
            default:= std::datetime_current();
        }
        required status: BlogPostStatus{
            default:= BlogPostStatus.Active;
        }
    }
}
