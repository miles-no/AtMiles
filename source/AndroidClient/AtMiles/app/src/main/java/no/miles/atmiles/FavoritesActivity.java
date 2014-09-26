package no.miles.atmiles;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.Toast;


public class FavoritesActivity extends Activity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_favorites);

        // Show the Up button in the action bar.
        getActionBar().setDisplayHomeAsUpEnabled(true);
    }


    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.favorites, menu);
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
            case R.id.action_menu_profile:
                startActivity(new Intent(this, ProfileActivity.class));
                break;
            default:
                handled = super.onOptionsItemSelected(item);
        }
        return handled;
    }
}
