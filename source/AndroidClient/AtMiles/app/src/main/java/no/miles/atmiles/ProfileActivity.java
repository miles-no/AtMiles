package no.miles.atmiles;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.Toast;


public class ProfileActivity extends Activity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_profile);

        // Show the Up button in the action bar.
        getActionBar().setDisplayHomeAsUpEnabled(true);

        new AuthenticationHelper().checkLogin(this);
    }


    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.profile, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        boolean handled = true;
        int id = item.getItemId();

        switch(id){
            case android.R.id.home:
                navigateUpTo(new Intent(this, EmployeeListActivity.class));
                break;
            case R.id.action_menu_favorites:
                startActivity(new Intent(this, FavoritesActivity.class));
                break;
            default:
                handled = super.onOptionsItemSelected(item);
        }
        return handled;
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent authActivityResult) {
        super.onActivityResult(requestCode, resultCode, authActivityResult);

        switch(requestCode)
        {
            case AuthenticationActivity.AUTH_REQUEST_COMPLETE:
                if(resultCode!=RESULT_OK)
                {
                    finish();
                }
                break;
        }
    }
}
