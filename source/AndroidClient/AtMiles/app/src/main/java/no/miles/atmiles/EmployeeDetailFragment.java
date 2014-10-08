package no.miles.atmiles;

import android.app.Fragment;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.AsyncTask;
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
import android.widget.Toast;

import com.fasterxml.jackson.databind.ObjectMapper;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLEncoder;

import no.miles.atmiles.employee.EmployeeDetailsResponse;
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

    /**
     * The dummy content this fragment is presenting.
     */
    private EmployeeDetailsResponse mItem;

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
        if (getArguments().containsKey(ARG_ITEM_ID)) {

            String employeeId = getArguments().getString(ARG_ITEM_ID);
            final String token = new AuthenticationHelper().getJsonWebToken(getActivity());

            //TODO: Get base from a more central place
            String searchUrl = "http://milescontact.cloudapp.net/api/company/miles/employee/" + URLEncoder.encode(employeeId);

            //TODO: Save reference to be able to cancel if new search is started before the previous is completed
            new CallSearchApi().execute(searchUrl, token);
            //TODO: Download data
            mItem = new EmployeeDetailsResponse();
            mItem.GlobalId= employeeId;
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
        if(mItem != null) {
            Intent contactIntent = new Intent(Intent.ACTION_INSERT_OR_EDIT);
            contactIntent.setType(ContactsContract.Contacts.CONTENT_ITEM_TYPE);

            if(mItem.Email != null) {
                contactIntent.putExtra(ContactsContract.Intents.Insert.EMAIL, mItem.Email);
                contactIntent.putExtra(ContactsContract.Intents.Insert.EMAIL_TYPE, ContactsContract.CommonDataKinds.Email.TYPE_WORK);
            }

            if(mItem.Name != null) {
                contactIntent.putExtra(ContactsContract.Intents.Insert.NAME, mItem.Name);
            }

            if(mItem.JobTitle != null) {
                contactIntent.putExtra(ContactsContract.Intents.Insert.JOB_TITLE, mItem.JobTitle);
            }

            if(mItem.PhoneNumber != null) {
                contactIntent.putExtra(ContactsContract.Intents.Insert.PHONE, mItem.PhoneNumber);
                contactIntent.putExtra(ContactsContract.Intents.Insert.PHONE_TYPE, ContactsContract.CommonDataKinds.Phone.TYPE_WORK);
            }

            if(mItem.CompanyId != null) {
                contactIntent.putExtra(ContactsContract.Intents.Insert.COMPANY, mItem.CompanyId);
            }

            if(mItem.OfficeName != null) {
                String notes = "Office: " + mItem.OfficeName;
                contactIntent.putExtra(ContactsContract.Intents.Insert.NOTES, notes);
            }

            startActivity(contactIntent);
        }
        else{
            Toast.makeText(getActivity(),"No data set", Toast.LENGTH_LONG).show();
        }
    }

    private void transferToPhone() {
        if(mItem != null && mItem.PhoneNumber != null) {
            String phoneNumber = mItem.PhoneNumber;
            Intent callIntent = new Intent(Intent.ACTION_DIAL);
            callIntent.setData(Uri.parse("tel:" + Uri.encode(phoneNumber.trim())));
            startActivity(callIntent);
        }
        else{
            Toast.makeText(getActivity(),"No PhoneNumber set", Toast.LENGTH_LONG).show();
        }
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState) {
        View rootView = inflater.inflate(R.layout.fragment_employee_detail, container, false);

        // Show the dummy content as text in a TextView.
        if (mItem != null) {
            updateDataInView(rootView);
        }

        return rootView;
    }

    private void updateDataInView(View rootView) {
        if(mItem != null) {
            if(mItem.Name != null) {
                ((TextView) rootView.findViewById(R.id.employee_detail)).setText(mItem.Name);
            }
            if (mItem.Thumb != null) {
                Bitmap decodedByte = ConvertToImage(mItem.Thumb);
                ((ImageView) rootView.findViewById(R.id.employee_thumbnail)).setImageBitmap(decodedByte);
            }
        }
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

    private void updateDataOnUIThread(final EmployeeDetailsResponse data) {
        getActivity().runOnUiThread(new Runnable() {
            public void run() {
                mItem = data;
                if (mItem != null) {
                    Bitmap decodedByte = ConvertToImage(mItem.Thumb);

                    ((ImageView) getActivity().findViewById(R.id.employee_thumbnail)).setImageBitmap(decodedByte);
                    ((TextView) getActivity().findViewById(R.id.employee_detail)).setText(mItem.Name);
                }
            }
        });
    }

    private class CallSearchApi extends AsyncTask<String, String, String> {

        @Override
        protected String doInBackground(String... params) {
            String urlString = params[0]; // URL to call
            String token = params[1]; // JWT

            String resultToDisplay = "";
            InputStream in = null;
            try {
                URL url = new URL(urlString);
                HttpURLConnection urlConnection = (HttpURLConnection) url.openConnection();
                urlConnection.setRequestProperty("Authorization", "Bearer " + token);
                in = new BufferedInputStream(urlConnection.getInputStream());

                BufferedReader reader = new BufferedReader(new InputStreamReader(in));
                StringBuilder out = new StringBuilder();
                String line;
                while ((line = reader.readLine()) != null) {
                    out.append(line);
                }
                resultToDisplay = out.toString();
                reader.close();
            } catch (Exception e) {
                //TODO: Log better
                System.out.println(e.getMessage());
                return e.getMessage();
            }
            return resultToDisplay;
        }

        protected void onPostExecute(String result) {
            ObjectMapper mapper = new ObjectMapper();
            EmployeeDetailsResponse resultObject = null;
            try {
                resultObject = mapper.readValue(result, EmployeeDetailsResponse.class);
            } catch (IOException e) {
                //TODO: Log better
                e.printStackTrace();
            }
            updateDataOnUIThread(resultObject);
        }
    }
}
