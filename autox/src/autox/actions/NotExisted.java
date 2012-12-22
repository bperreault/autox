package autox.actions;

/**
 * Created with AutoX project.
 * User: jien.huang
 * Date: 12/15/12
 */
public class NotExisted extends Action {
    @Override
    public Object findTestObject() {
        return NotExpectedFindUIObject();
    }

    @Override
    protected void handle(Object testObject) {
        if(testObject!=null){
            getResult().Error("The not expected object is existed!");
            return;
        }

    }
}
