package no.miles.atmiles;

import android.app.Fragment;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Bundle;
import android.provider.ContactsContract;
import android.util.Base64;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import java.io.ByteArrayInputStream;
import java.io.InputStream;

import no.miles.atmiles.employee.SearchResultModel;

/**
 * A fragment representing a single Employee detail screen.
 * This fragment is either contained in a {@link EmployeeListActivity}
 * in two-pane mode (on tablets) or a {@link EmployeeDetailActivity}
 * on handsets.
 */
public class EmployeeDetailFragment extends Fragment {
    /**
     * The fragment argument representing the item ID that this fragment
     * represents.
     */
    public static final String ARG_ITEM_ID = "item_id";
    public static final String ARG_ITEM = "item";

    /**
     * The dummy content this fragment is presenting.
     */
    private SearchResultModel.Result mItem;

    /**
     * Mandatory empty constructor for the fragment manager to instantiate the
     * fragment (e.g. upon screen orientation changes).
     */
    public EmployeeDetailFragment() {
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setHasOptionsMenu(true);
        if (getArguments().containsKey(ARG_ITEM)) {
            mItem = (SearchResultModel.Result)getArguments().get(ARG_ITEM);
        }
    }

    @Override
    public void onCreateOptionsMenu(Menu menu, MenuInflater inflater) {
        boolean call = true;
        //TODO: Check if employee has phone-number
        if(call) {
            getActivity().getMenuInflater().inflate(R.menu.call_employee, menu);
        }
        getActivity().getMenuInflater().inflate(R.menu.add_employee_to_contacts, menu);
    }


    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        boolean handled = true;
        int id = item.getItemId();

        switch(id){
            case R.id.action_menu_call_employee:
                transferToPhone();
                break;
            case R.id.action_menu_add_employee_to_contatcs:
                addToContacts();
                break;
            default:
                handled = super.onOptionsItemSelected(item);
        }
        return handled;
    }

    private void addToContacts() {
        //TODO: Get info from employee
        String email = "roy.veshovda@miles.no";
        String name = "Roy Veshovda";
        String title = "Senior Consultant";
        String company = "Miles";
        String notes = "Office: Stavanger";


        Intent contactIntent = new Intent(Intent.ACTION_INSERT_OR_EDIT);
        contactIntent.setType(ContactsContract.Contacts.CONTENT_ITEM_TYPE);
        contactIntent.putExtra(ContactsContract.Intents.Insert.EMAIL, email);
        contactIntent.putExtra(ContactsContract.Intents.Insert.EMAIL_TYPE, ContactsContract.CommonDataKinds.Email.TYPE_WORK);
        contactIntent.putExtra(ContactsContract.Intents.Insert.NAME, name);
        contactIntent.putExtra(ContactsContract.Intents.Insert.JOB_TITLE, title);
        contactIntent.putExtra(ContactsContract.Intents.Insert.PHONE_TYPE, ContactsContract.CommonDataKinds.Phone.TYPE_WORK);
        contactIntent.putExtra(ContactsContract.Intents.Insert.COMPANY, company);
        contactIntent.putExtra(ContactsContract.Intents.Insert.NOTES, notes);
        startActivity(contactIntent);
    }

    private void transferToPhone() {
        //TODO: call actuall phone-number for employee
        String phoneNumber = "+4740102040";
        Intent callIntent = new Intent(Intent.ACTION_DIAL);
        callIntent.setData(Uri.parse("tel:"+Uri.encode(phoneNumber.trim())));
        startActivity(callIntent);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState) {
        View rootView = inflater.inflate(R.layout.fragment_employee_detail, container, false);

        // Show the dummy content as text in a TextView.
        if (mItem != null) {
            ((TextView) rootView.findViewById(R.id.employee_detail)).setText(mItem.GlobalId + " " + mItem.Name);

            Bitmap decodedByte = ConvertToImage(mItem.Thumb);
            ((ImageView)rootView.findViewById(R.id.employee_thumbnail)).setImageBitmap(decodedByte);
        }

        return rootView;
    }

    public static Bitmap ConvertToImage(String image){
        try{
            String imageDataBytes = image.substring(image.indexOf(",")+1);
            InputStream stream = new ByteArrayInputStream(Base64.decode(imageDataBytes.getBytes(), Base64.DEFAULT));
            Bitmap bitmap = BitmapFactory.decodeStream(stream);
            return bitmap;
        }
        catch (Exception e) {
            Log.e("Convert", e.getMessage());
            return null;
        }
    }
}
