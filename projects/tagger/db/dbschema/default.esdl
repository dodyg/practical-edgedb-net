module default {
    scalar type ObjectStatus extending enum<Active, Deleted, ForReview>;

    type Namespace {
        required name : str;
        required status: ObjectStatus {
            default := ObjectStatus.ForReview;
        }
    }

    type Tag {
        required name : str{
            constraint min_len_value(2);
            constraint max_len_value(50);
        }

        description : str;  
        required ns : Namespace;
        required status: ObjectStatus {
            default := ObjectStatus.Active;
        }
    }

    type Resource {
        required title : str;
        required url : str;
        multi tags : Tag;
        required status: ObjectStatus {
            default := ObjectStatus.Active;
        }
    }
}
